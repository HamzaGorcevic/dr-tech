using System.Net;
using System.Text.Json;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();
            var statusCode = HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case ArgumentNullException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = "Invalid request parameters";
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = "Invalid request parameters";
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Unauthorized access";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorResponse.Message = "Resource not found";
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.Conflict;
                    errorResponse.Message = "Operation not allowed";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred";
                    break;
            }

            response.StatusCode = (int)statusCode;
            errorResponse.StatusCode = (int)statusCode;
            errorResponse.Timestamp = DateTime.UtcNow;
            errorResponse.Path = context.Request.Path;
            errorResponse.Method = context.Request.Method;

            // Log error to database
            await LogErrorAsync(context, exception, (int)statusCode);

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }

        private async Task LogErrorAsync(HttpContext context, Exception exception, int statusCode)
        {
            try
            {
                var userId = context.User?.FindFirst("sub")?.Value ?? "anonymous";
                var errorType = statusCode >= 400 && statusCode < 500 ? "4xx" : "5xx";

                var errorLog = new Domain.Entities.ErrorLog
                {
                    Id = Guid.NewGuid(),
                    ErrorType = errorType,
                    StatusCode = statusCode,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    RequestPath = context.Request.Path,
                    RequestMethod = context.Request.Method,
                    UserId = userId,
                    OccurredAtUtc = DateTime.UtcNow
                };

                // Use service locator to get the scoped service
                var errorLogDb = context.RequestServices.GetRequiredService<IDatabaseService<Domain.Entities.ErrorLog>>();
                await errorLogDb.AddAsync(errorLog, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log error to database");
            }
        }

        private class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public int StatusCode { get; set; }
            public DateTime Timestamp { get; set; }
            public string Path { get; set; } = string.Empty;
            public string Method { get; set; } = string.Empty;
        }
    }
}