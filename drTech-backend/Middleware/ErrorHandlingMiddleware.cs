using System.Net;
using System.Text.Json;
using drTech_backend.Infrastructure.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
				case DbUpdateException dbex when dbex.InnerException is PostgresException pex:
					// Map common Postgres error codes
					if (pex.SqlState == "23505") // unique_violation
					{
						statusCode = HttpStatusCode.Conflict;
						errorResponse.Message = "Unique constraint violation";
						var field = InferFieldFromConstraint(pex.ConstraintName);
						if (!string.IsNullOrEmpty(field))
						{
							errorResponse.Errors = new List<ValidationError>
							{
								new ValidationError
								{
									Field = field,
									Messages = new List<string> { "Duplicate value not allowed" }
								}
							};
						}
					}
					else if (pex.SqlState == "23503") // foreign_key_violation
					{
						statusCode = HttpStatusCode.BadRequest;
						errorResponse.Message = "Invalid reference to related entity";
					}
					else
					{
						statusCode = HttpStatusCode.BadRequest;
						errorResponse.Message = "Database constraint violation";
					}
					break;
				case ValidationException vex:
					statusCode = HttpStatusCode.BadRequest;
					errorResponse.Message = "Validation failed";
					errorResponse.Errors = vex.Errors
						.GroupBy(e => e.PropertyName)
						.Select(g => new ValidationError
						{
							Field = g.Key,
							Messages = g.Select(e => e.ErrorMessage).Distinct().ToList()
						})
						.ToList();
					break;
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

		private static string InferFieldFromConstraint(string? constraintName)
		{
			if (string.IsNullOrWhiteSpace(constraintName)) return string.Empty;
			// Simple mapping for known unique indexes
			if (constraintName.Contains("IX_Doctors_UserId", StringComparison.OrdinalIgnoreCase)) return "UserId";
			if (constraintName.Contains("IX_Patients_UserId", StringComparison.OrdinalIgnoreCase)) return "UserId";
			if (constraintName.Contains("IX_InsuranceAgencies_UserId", StringComparison.OrdinalIgnoreCase)) return "UserId";
			if (constraintName.Contains("IX_Hospitals_UserId", StringComparison.OrdinalIgnoreCase)) return "UserId";
			if (constraintName.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase)) return "Email";
			return string.Empty;
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
			public List<ValidationError>? Errors { get; set; }
        }

		private class ValidationError
		{
			public string Field { get; set; } = string.Empty;
			public List<string> Messages { get; set; } = new List<string>();
		}
    }
}