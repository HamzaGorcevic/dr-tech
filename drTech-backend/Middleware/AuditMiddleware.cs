using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(
            RequestDelegate next,
            ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            // Capture request information
            var requestInfo = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.Value,
                UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                IpAddress = GetClientIpAddress(context),
                UserId = context.User?.FindFirst("sub")?.Value ?? "anonymous",
                UserRole = context.User?.FindFirst("role")?.Value ?? "anonymous"
            };

            await _next(context);

            // Log audit information for data-changing operations
            if (ShouldAudit(context))
            {
                await LogAuditAsync(context, requestInfo, startTime);
            }
        }

        private bool ShouldAudit(HttpContext context)
        {
            var method = context.Request.Method.ToUpper();
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Audit data-changing operations
            if (method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH")
            {
                // Skip certain paths that don't need auditing
                var skipPaths = new[]
                {
                    "/health",
                    "/metrics",
                    "/swagger",
                    "/favicon.ico",
                    "/api/auth/login",
                    "/api/auth/refresh"
                };

                return !skipPaths.Any(skipPath => path.StartsWith(skipPath));
            }

            return false;
        }

        private async Task LogAuditAsync(HttpContext context, object requestInfo, DateTime startTime)
        {
            try
            {
                var action = GetActionFromMethod(context.Request.Method);
                var actor = context.User?.FindFirst("sub")?.Value ?? "anonymous";
                var description = GenerateDescription(context, requestInfo);

                var auditLog = new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Actor = actor,
                    Action = action,
                    Path = context.Request.Path.Value ?? "",
                    Method = context.Request.Method,
                    StatusCode = context.Response.StatusCode,
                    OccurredAtUtc = startTime,
                    Description = description
                };

                // Use service locator to get the scoped service
                var auditLogDb = context.RequestServices.GetRequiredService<IDatabaseService<Domain.Entities.AuditLog>>();
                await auditLogDb.AddAsync(auditLog, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit information");
            }
        }

        private string GetActionFromMethod(string method)
        {
            return method.ToUpper() switch
            {
                "POST" => "Create",
                "PUT" => "Update",
                "PATCH" => "Update",
                "DELETE" => "Delete",
                _ => "Unknown"
            };
        }

        private string GenerateDescription(HttpContext context, object requestInfo)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "";
            var statusCode = context.Response.StatusCode;

            var description = $"{method} {path} - Status: {statusCode}";

            // Add more context based on the endpoint
            if (path.Contains("/hospitals"))
            {
                description += " - Hospital management";
            }
            else if (path.Contains("/doctors"))
            {
                description += " - Doctor management";
            }
            else if (path.Contains("/patients"))
            {
                description += " - Patient management";
            }
            else if (path.Contains("/appointments"))
            {
                description += " - Appointment management";
            }
            else if (path.Contains("/equipment"))
            {
                description += " - Equipment management";
            }
            else if (path.Contains("/contracts"))
            {
                description += " - Contract management";
            }
            else if (path.Contains("/payments"))
            {
                description += " - Payment management";
            }
            else if (path.Contains("/discounts"))
            {
                description += " - Discount management";
            }

            return description;
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