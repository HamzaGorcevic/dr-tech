using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace drTech_backend.Middleware
{
    public class ThrottlingOptions
    {
        public int Limit { get; set; } = 100;
        public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan BlockDuration { get; set; } = TimeSpan.FromMinutes(5);
        public HashSet<string> StrictPaths { get; set; } = new(new[] { "/api/uploads", "/api/contracts" });
        public int StrictLimit { get; set; } = 20;
        public TimeSpan StrictWindow { get; set; } = TimeSpan.FromMinutes(10);
    }

    internal class ClientState
    {
        public DateTime WindowStartUtc { get; set; }
        public int Count { get; set; }
        public DateTime? BlockedUntilUtc { get; set; }
    }

    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ThrottlingMiddleware> _logger;
        private readonly ThrottlingOptions _options;
        private static readonly ConcurrentDictionary<string, ClientState> Clients = new();

        public ThrottlingMiddleware(RequestDelegate next, ILogger<ThrottlingMiddleware> logger, IOptions<ThrottlingOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var key = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.Value ?? string.Empty;
            var now = DateTime.UtcNow;
            var isStrict = _options.StrictPaths.Contains(path);

            var limit = isStrict ? _options.StrictLimit : _options.Limit;
            var window = isStrict ? _options.StrictWindow : _options.Window;

            var state = Clients.GetOrAdd(key, _ => new ClientState { WindowStartUtc = now, Count = 0 });

            lock (state)
            {
                if (state.BlockedUntilUtc.HasValue && state.BlockedUntilUtc.Value > now)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["Retry-After"] = ((int)(state.BlockedUntilUtc.Value - now).TotalSeconds).ToString();
                }
                else
                {
                    if (now - state.WindowStartUtc > window)
                    {
                        state.WindowStartUtc = now;
                        state.Count = 0;
                    }
                    state.Count++;
                    if (state.Count > limit)
                    {
                        state.BlockedUntilUtc = now.Add(_options.BlockDuration);
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.Headers["Retry-After"] = ((int)_options.BlockDuration.TotalSeconds).ToString();
                    }
                }
            }

            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                _logger.LogWarning("Throttled request from {Key} to {Path}", key, path);
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}


