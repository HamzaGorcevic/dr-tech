using System.Collections.Concurrent;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Middleware
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ThrottlingMiddleware> _logger;
        private readonly ConcurrentDictionary<string, ThrottleInfo> _throttleCache = new();

        // Configuration
        private readonly int _maxRequestsPerWindow = 100;
        private readonly int _windowSizeMinutes = 10;
        private readonly int _blockDurationMinutes = 5;

        public ThrottlingMiddleware(
            RequestDelegate next,
            ILogger<ThrottlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientId(context);
            var now = DateTime.UtcNow;

            // Check if client is currently blocked
            if (IsClientBlocked(clientId, now))
            {
                _logger.LogWarning("Request blocked for client {ClientId} - still in block period", clientId);
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // Get or create throttle info for this client
            var throttleInfo = _throttleCache.GetOrAdd(clientId, _ => new ThrottleInfo
            {
                WindowStart = now,
                RequestCount = 0
            });

            // Check if we need to reset the window
            if (now - throttleInfo.WindowStart > TimeSpan.FromMinutes(_windowSizeMinutes))
            {
                throttleInfo.WindowStart = now;
                throttleInfo.RequestCount = 0;
            }

            // Increment request count
            throttleInfo.RequestCount++;

            // Check if limit exceeded
            if (throttleInfo.RequestCount > _maxRequestsPerWindow)
            {
                _logger.LogWarning("Rate limit exceeded for client {ClientId}: {RequestCount} requests in window", 
                    clientId, throttleInfo.RequestCount);

                // Block the client
                throttleInfo.IsBlocked = true;
                throttleInfo.BlockedUntil = now.AddMinutes(_blockDurationMinutes);

                // Log the throttle event
                await LogThrottleEventAsync(context, clientId, throttleInfo, now);

                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // Add rate limit headers
            context.Response.Headers["X-RateLimit-Limit"] = _maxRequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = (_maxRequestsPerWindow - throttleInfo.RequestCount).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = throttleInfo.WindowStart.AddMinutes(_windowSizeMinutes).ToString("R");

            await _next(context);
        }

        private bool IsClientBlocked(string clientId, DateTime now)
        {
            if (_throttleCache.TryGetValue(clientId, out var throttleInfo))
            {
                if (throttleInfo.IsBlocked && throttleInfo.BlockedUntil > now)
                {
                    return true;
                }
                else if (throttleInfo.IsBlocked && throttleInfo.BlockedUntil <= now)
                {
                    // Unblock the client
                    throttleInfo.IsBlocked = false;
                    throttleInfo.BlockedUntil = null;
                    throttleInfo.WindowStart = now;
                    throttleInfo.RequestCount = 0;
                }
            }

            return false;
        }

        private async Task LogThrottleEventAsync(HttpContext context, string clientId, ThrottleInfo throttleInfo, DateTime now)
        {
            try
            {
                var throttleLog = new Domain.Entities.ThrottleLog
                {
                    Id = Guid.NewGuid(),
                    UserId = clientId.Contains("@") ? clientId : "anonymous",
                    IpAddress = clientId.Contains(".") ? clientId : "unknown",
                    RequestCount = throttleInfo.RequestCount,
                    WindowStartUtc = throttleInfo.WindowStart,
                    WindowEndUtc = throttleInfo.WindowStart.AddMinutes(_windowSizeMinutes),
                    IsBlocked = true,
                    BlockedUntilUtc = throttleInfo.BlockedUntil
                };

                // Use service locator to get the scoped service
                var throttleLogDb = context.RequestServices.GetRequiredService<IDatabaseService<Domain.Entities.ThrottleLog>>();
                await throttleLogDb.AddAsync(throttleLog, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log throttle event to database");
            }
        }

        private string GetClientId(HttpContext context)
        {
            // Try to get user ID first
            var userId = context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            // Fall back to IP address
            var ipAddress = GetClientIpAddress(context);
            return ipAddress;
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (for load balancers/proxies)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Check for real IP header
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fall back to connection remote IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private class ThrottleInfo
        {
            public DateTime WindowStart { get; set; }
            public int RequestCount { get; set; }
            public bool IsBlocked { get; set; }
            public DateTime? BlockedUntil { get; set; }
        }
    }
}