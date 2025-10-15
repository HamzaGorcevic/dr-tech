using System;

namespace drTech_backend.Application.Common.DTOs
{
    public class ErrorLogDto
    {
        public Guid Id { get; set; }
        public string ErrorType { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public string? UserId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
    }

    public class RequestLogDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    public class ThrottleLogDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public DateTime WindowStartUtc { get; set; }
        public DateTime WindowEndUtc { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntilUtc { get; set; }
    }

    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string Actor { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string? Description { get; set; }
    }
}


