using System.Diagnostics;

namespace drTech_backend.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var user = context.User?.Identity?.Name ?? "anonymous";
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path;
            var method = context.Request.Method;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response?.StatusCode;
                _logger.LogInformation("HTTP {Method} {Path} by {User} from {IP} responded {StatusCode} in {Elapsed} ms",
                    method, path, user, ip, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}


