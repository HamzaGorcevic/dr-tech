using System.Diagnostics;
using System.Text.Json;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            // Log request start
            _logger.LogInformation("Request {RequestId} started: {Method} {Path} from {RemoteIp}",
                requestId, context.Request.Method, context.Request.Path, GetClientIpAddress(context));

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log request completion
                _logger.LogInformation("Request {RequestId} completed: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                    requestId, context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);

                // Store request log in database
                await LogRequestAsync(context, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task LogRequestAsync(HttpContext context, long responseTimeMs)
        {
            try
            {
                var userId = context.User?.FindFirst("sub")?.Value ?? "anonymous";
                var ipAddress = GetClientIpAddress(context);

                var requestLog = new Domain.Entities.RequestLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IpAddress = ipAddress,
                    Endpoint = context.Request.Path,
                    HttpMethod = context.Request.Method,
                    StatusCode = context.Response.StatusCode,
                    ResponseTimeMs = responseTimeMs,
                    TimestampUtc = DateTime.UtcNow
                };

                // Use service locator to get the scoped service
                var requestLogDb = context.RequestServices.GetRequiredService<IDatabaseService<Domain.Entities.RequestLog>>();
                await requestLogDb.AddAsync(requestLog, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log request to database");
            }
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
    }
}