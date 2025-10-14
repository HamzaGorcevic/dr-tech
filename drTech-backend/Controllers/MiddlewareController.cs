using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HospitalAdmin,InsuranceAgency")]
    public class MiddlewareController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabaseService<Domain.Entities.RequestLog> _requestLogDb;
        private readonly IDatabaseService<Domain.Entities.ThrottleLog> _throttleLogDb;

        public MiddlewareController(
            IMediator mediator,
            IDatabaseService<Domain.Entities.RequestLog> requestLogDb,
            IDatabaseService<Domain.Entities.ThrottleLog> throttleLogDb)
        {
            _mediator = mediator;
            _requestLogDb = requestLogDb;
            _throttleLogDb = throttleLogDb;
        }

        [HttpGet("request-logs")]
        public async Task<IActionResult> GetRequestLogs([FromQuery] RequestLogFilterDto filter, CancellationToken cancellationToken)
        {
            var requestLogs = await _requestLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (!string.IsNullOrEmpty(filter.UserId))
                requestLogs = requestLogs.Where(log => log.UserId == filter.UserId).ToList();
            
            if (!string.IsNullOrEmpty(filter.IpAddress))
                requestLogs = requestLogs.Where(log => log.IpAddress == filter.IpAddress).ToList();
            
            if (!string.IsNullOrEmpty(filter.Endpoint))
                requestLogs = requestLogs.Where(log => log.Endpoint.Contains(filter.Endpoint, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(filter.HttpMethod))
                requestLogs = requestLogs.Where(log => log.HttpMethod == filter.HttpMethod).ToList();
            
            if (filter.StatusCode.HasValue)
                requestLogs = requestLogs.Where(log => log.StatusCode == filter.StatusCode.Value).ToList();
            
            if (filter.MinResponseTime.HasValue)
                requestLogs = requestLogs.Where(log => log.ResponseTimeMs >= filter.MinResponseTime.Value).ToList();
            
            if (filter.MaxResponseTime.HasValue)
                requestLogs = requestLogs.Where(log => log.ResponseTimeMs <= filter.MaxResponseTime.Value).ToList();
            
            if (filter.StartDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc >= filter.StartDate.Value).ToList();
            
            if (filter.EndDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc <= filter.EndDate.Value).ToList();

            // Order by most recent first
            requestLogs = requestLogs.OrderByDescending(log => log.TimestampUtc).ToList();

            // Apply pagination
            if (filter.PageSize > 0)
            {
                var skip = (filter.Page - 1) * filter.PageSize;
                requestLogs = requestLogs.Skip(skip).Take(filter.PageSize).ToList();
            }

            return Ok(requestLogs);
        }

        [HttpGet("throttle-logs")]
        public async Task<IActionResult> GetThrottleLogs([FromQuery] ThrottleLogFilterDto filter, CancellationToken cancellationToken)
        {
            var throttleLogs = await _throttleLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (!string.IsNullOrEmpty(filter.UserId))
                throttleLogs = throttleLogs.Where(log => log.UserId == filter.UserId).ToList();
            
            if (!string.IsNullOrEmpty(filter.IpAddress))
                throttleLogs = throttleLogs.Where(log => log.IpAddress == filter.IpAddress).ToList();
            
            if (filter.IsBlocked.HasValue)
                throttleLogs = throttleLogs.Where(log => log.IsBlocked == filter.IsBlocked.Value).ToList();
            
            if (filter.StartDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowStartUtc >= filter.StartDate.Value).ToList();
            
            if (filter.EndDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowEndUtc <= filter.EndDate.Value).ToList();

            // Order by most recent first
            throttleLogs = throttleLogs.OrderByDescending(log => log.WindowStartUtc).ToList();

            // Apply pagination
            if (filter.PageSize > 0)
            {
                var skip = (filter.Page - 1) * filter.PageSize;
                throttleLogs = throttleLogs.Skip(skip).Take(filter.PageSize).ToList();
            }

            return Ok(throttleLogs);
        }

        [HttpGet("request-logs/summary")]
        public async Task<IActionResult> GetRequestLogSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
        {
            var requestLogs = await _requestLogDb.GetAllAsync(cancellationToken);
            
            if (startDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc <= endDate.Value).ToList();

            var summary = new RequestLogSummaryDto
            {
                TotalRequests = requestLogs.Count(),
                SuccessfulRequests = requestLogs.Count(log => log.StatusCode >= 200 && log.StatusCode < 300),
                ClientErrors = requestLogs.Count(log => log.StatusCode >= 400 && log.StatusCode < 500),
                ServerErrors = requestLogs.Count(log => log.StatusCode >= 500),
                AverageResponseTime = requestLogs.Any() ? requestLogs.Average(log => log.ResponseTimeMs) : 0,
                TopEndpoints = requestLogs
                    .GroupBy(log => log.Endpoint)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { Endpoint = g.Key, Count = g.Count(), AvgResponseTime = g.Average(l => l.ResponseTimeMs) }),
                TopUsers = requestLogs
                    .Where(log => !string.IsNullOrEmpty(log.UserId))
                    .GroupBy(log => log.UserId)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { UserId = g.Key, Count = g.Count() }),
                TopIpAddresses = requestLogs
                    .GroupBy(log => log.IpAddress)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { IpAddress = g.Key, Count = g.Count() }),
                StatusCodeDistribution = requestLogs
                    .GroupBy(log => log.StatusCode)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { StatusCode = g.Key, Count = g.Count() })
            };

            return Ok(summary);
        }

        [HttpGet("throttle-logs/summary")]
        public async Task<IActionResult> GetThrottleLogSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
        {
            var throttleLogs = await _throttleLogDb.GetAllAsync(cancellationToken);
            
            if (startDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowStartUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowEndUtc <= endDate.Value).ToList();

            var summary = new ThrottleLogSummaryDto
            {
                TotalThrottleEvents = throttleLogs.Count(),
                BlockedEvents = throttleLogs.Count(log => log.IsBlocked),
                AverageRequestCount = throttleLogs.Any() ? throttleLogs.Average(log => log.RequestCount) : 0,
                TopThrottledUsers = throttleLogs
                    .Where(log => !string.IsNullOrEmpty(log.UserId))
                    .GroupBy(log => log.UserId)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { UserId = g.Key, ThrottleCount = g.Count(), MaxRequests = g.Max(l => l.RequestCount) }),
                TopThrottledIpAddresses = throttleLogs
                    .GroupBy(log => log.IpAddress)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { IpAddress = g.Key, ThrottleCount = g.Count(), MaxRequests = g.Max(l => l.RequestCount) })
            };

            return Ok(summary);
        }

        [HttpGet("request-logs/trends")]
        public async Task<IActionResult> GetRequestTrends([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken, [FromQuery] string groupBy = "hour")
        {
            var requestLogs = await _requestLogDb.GetAllAsync(cancellationToken);
            
            if (startDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc <= endDate.Value).ToList();

            var trends = groupBy.ToLower() switch
            {
                "minute" => requestLogs.GroupBy(log => new { log.TimestampUtc.Year, log.TimestampUtc.Month, log.TimestampUtc.Day, log.TimestampUtc.Hour, log.TimestampUtc.Minute })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2} {g.Key.Hour:D2}:{g.Key.Minute:D2}", Count = g.Count(), AvgResponseTime = g.Average(l => l.ResponseTimeMs) }),
                "hour" => requestLogs.GroupBy(log => new { log.TimestampUtc.Year, log.TimestampUtc.Month, log.TimestampUtc.Day, log.TimestampUtc.Hour })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2} {g.Key.Hour:D2}:00", Count = g.Count(), AvgResponseTime = g.Average(l => l.ResponseTimeMs) }),
                "day" => requestLogs.GroupBy(log => new { log.TimestampUtc.Year, log.TimestampUtc.Month, log.TimestampUtc.Day })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}", Count = g.Count(), AvgResponseTime = g.Average(l => l.ResponseTimeMs) }),
                _ => requestLogs.GroupBy(log => new { log.TimestampUtc.Year, log.TimestampUtc.Month, log.TimestampUtc.Day })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}", Count = g.Count(), AvgResponseTime = g.Average(l => l.ResponseTimeMs) })
            };

            return Ok(trends.OrderBy(t => t.Period));
        }

        [HttpPost("request-logs/export")]
        public async Task<IActionResult> ExportRequestLogs([FromBody] RequestLogExportDto request, CancellationToken cancellationToken)
        {
            var requestLogs = await _requestLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (request.StartDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc >= request.StartDate.Value).ToList();
            
            if (request.EndDate.HasValue)
                requestLogs = requestLogs.Where(log => log.TimestampUtc <= request.EndDate.Value).ToList();
            
            if (!string.IsNullOrEmpty(request.UserId))
                requestLogs = requestLogs.Where(log => log.UserId == request.UserId).ToList();
            
            if (!string.IsNullOrEmpty(request.IpAddress))
                requestLogs = requestLogs.Where(log => log.IpAddress == request.IpAddress).ToList();

            var csv = GenerateRequestLogCsv(requestLogs.OrderByDescending(log => log.TimestampUtc));
            
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"request_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }

        [HttpPost("throttle-logs/export")]
        public async Task<IActionResult> ExportThrottleLogs([FromBody] ThrottleLogExportDto request, CancellationToken cancellationToken)
        {
            var throttleLogs = await _throttleLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (request.StartDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowStartUtc >= request.StartDate.Value).ToList();
            
            if (request.EndDate.HasValue)
                throttleLogs = throttleLogs.Where(log => log.WindowEndUtc <= request.EndDate.Value).ToList();
            
            if (!string.IsNullOrEmpty(request.UserId))
                throttleLogs = throttleLogs.Where(log => log.UserId == request.UserId).ToList();
            
            if (!string.IsNullOrEmpty(request.IpAddress))
                throttleLogs = throttleLogs.Where(log => log.IpAddress == request.IpAddress).ToList();

            var csv = GenerateThrottleLogCsv(throttleLogs.OrderByDescending(log => log.WindowStartUtc));
            
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"throttle_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }

        private string GenerateRequestLogCsv(IEnumerable<Domain.Entities.RequestLog> logs)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Timestamp,UserId,IpAddress,Endpoint,HttpMethod,StatusCode,ResponseTimeMs");
            
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.TimestampUtc:yyyy-MM-dd HH:mm:ss},{log.UserId},{log.IpAddress},{log.Endpoint},{log.HttpMethod},{log.StatusCode},{log.ResponseTimeMs}");
            }
            
            return csv.ToString();
        }

        private string GenerateThrottleLogCsv(IEnumerable<Domain.Entities.ThrottleLog> logs)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("WindowStart,WindowEnd,UserId,IpAddress,RequestCount,IsBlocked,BlockedUntil");
            
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.WindowStartUtc:yyyy-MM-dd HH:mm:ss},{log.WindowEndUtc:yyyy-MM-dd HH:mm:ss},{log.UserId},{log.IpAddress},{log.RequestCount},{log.IsBlocked},{log.BlockedUntilUtc:yyyy-MM-dd HH:mm:ss}");
            }
            
            return csv.ToString();
        }
    }

    public class RequestLogFilterDto
    {
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Endpoint { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public long? MinResponseTime { get; set; }
        public long? MaxResponseTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class ThrottleLogFilterDto
    {
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class RequestLogSummaryDto
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int ClientErrors { get; set; }
        public int ServerErrors { get; set; }
        public double AverageResponseTime { get; set; }
        public object TopEndpoints { get; set; } = new object();
        public object TopUsers { get; set; } = new object();
        public object TopIpAddresses { get; set; } = new object();
        public object StatusCodeDistribution { get; set; } = new object();
    }

    public class ThrottleLogSummaryDto
    {
        public int TotalThrottleEvents { get; set; }
        public int BlockedEvents { get; set; }
        public double AverageRequestCount { get; set; }
        public object TopThrottledUsers { get; set; } = new object();
        public object TopThrottledIpAddresses { get; set; } = new object();
    }

    public class RequestLogExportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
    }

    public class ThrottleLogExportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
    }
}
