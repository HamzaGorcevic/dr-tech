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
    public class ErrorLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabaseService<Domain.Entities.ErrorLog> _errorLogDb;

        public ErrorLogController(
            IMediator mediator,
            IDatabaseService<Domain.Entities.ErrorLog> errorLogDb)
        {
            _mediator = mediator;
            _errorLogDb = errorLogDb;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ErrorLogFilterDto filter, CancellationToken cancellationToken)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (!string.IsNullOrEmpty(filter.ErrorType))
                errorLogs = errorLogs.Where(log => log.ErrorType == filter.ErrorType).ToList();
            
            if (filter.StatusCode.HasValue)
                errorLogs = errorLogs.Where(log => log.StatusCode == filter.StatusCode.Value).ToList();
            
            if (!string.IsNullOrEmpty(filter.UserId))
                errorLogs = errorLogs.Where(log => log.UserId == filter.UserId).ToList();
            
            if (!string.IsNullOrEmpty(filter.RequestPath))
                errorLogs = errorLogs.Where(log => log.RequestPath?.Contains(filter.RequestPath, StringComparison.OrdinalIgnoreCase) == true).ToList();
            
            if (filter.StartDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc >= filter.StartDate.Value).ToList();
            
            if (filter.EndDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc <= filter.EndDate.Value).ToList();

            // Order by most recent first
            errorLogs = errorLogs.OrderByDescending(log => log.OccurredAtUtc).ToList();

            // Apply pagination
            if (filter.PageSize > 0)
            {
                var skip = (filter.Page - 1) * filter.PageSize;
                errorLogs = errorLogs.Skip(skip).Take(filter.PageSize).ToList();
            }

            return Ok(errorLogs);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var errorLog = await _mediator.Send(new GetByIdQuery<Domain.Entities.ErrorLog>(id), cancellationToken);
            return errorLog is null ? NotFound() : Ok(errorLog);
        }

        [HttpGet("type/{errorType}")]
        public async Task<IActionResult> GetByType(string errorType, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            var typeLogs = errorLogs
                .Where(log => log.ErrorType == errorType)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(typeLogs);
        }

        [HttpGet("status/{statusCode:int}")]
        public async Task<IActionResult> GetByStatusCode(int statusCode, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            var statusLogs = errorLogs
                .Where(log => log.StatusCode == statusCode)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(statusLogs);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            var userLogs = errorLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(userLogs);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            
            if (startDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc <= endDate.Value).ToList();

            var summary = new ErrorLogSummaryDto
            {
                TotalErrors = errorLogs.Count(),
                ClientErrors = errorLogs.Count(log => log.ErrorType == "4xx"),
                ServerErrors = errorLogs.Count(log => log.ErrorType == "5xx"),
                TopErrorTypes = errorLogs
                    .GroupBy(log => log.ErrorType)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { ErrorType = g.Key, Count = g.Count() }),
                TopStatusCodes = errorLogs
                    .GroupBy(log => log.StatusCode)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { StatusCode = g.Key, Count = g.Count() }),
                TopEndpoints = errorLogs
                    .Where(log => !string.IsNullOrEmpty(log.RequestPath))
                    .GroupBy(log => log.RequestPath)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { Endpoint = g.Key, Count = g.Count() }),
                TopUsers = errorLogs
                    .Where(log => !string.IsNullOrEmpty(log.UserId))
                    .GroupBy(log => log.UserId)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
            };

            return Ok(summary);
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken, [FromQuery] string groupBy = "day")
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            
            if (startDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc <= endDate.Value).ToList();

            var trends = groupBy.ToLower() switch
            {
                "hour" => errorLogs.GroupBy(log => new { log.OccurredAtUtc.Year, log.OccurredAtUtc.Month, log.OccurredAtUtc.Day, log.OccurredAtUtc.Hour })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2} {g.Key.Hour:D2}:00", Count = g.Count() }),
                "day" => errorLogs.GroupBy(log => new { log.OccurredAtUtc.Year, log.OccurredAtUtc.Month, log.OccurredAtUtc.Day })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}", Count = g.Count() }),
                "month" => errorLogs.GroupBy(log => new { log.OccurredAtUtc.Year, log.OccurredAtUtc.Month })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}", Count = g.Count() }),
                _ => errorLogs.GroupBy(log => new { log.OccurredAtUtc.Year, log.OccurredAtUtc.Month, log.OccurredAtUtc.Day })
                    .Select(g => new { Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}", Count = g.Count() })
            };

            return Ok(trends.OrderBy(t => t.Period));
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportErrorLogs([FromBody] ErrorLogExportDto request, CancellationToken cancellationToken)
        {
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            
            // Apply filters
            if (request.StartDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc >= request.StartDate.Value).ToList();
            
            if (request.EndDate.HasValue)
                errorLogs = errorLogs.Where(log => log.OccurredAtUtc <= request.EndDate.Value).ToList();
            
            if (!string.IsNullOrEmpty(request.ErrorType))
                errorLogs = errorLogs.Where(log => log.ErrorType == request.ErrorType).ToList();
            
            if (request.StatusCode.HasValue)
                errorLogs = errorLogs.Where(log => log.StatusCode == request.StatusCode.Value).ToList();

            var csv = GenerateCsv(errorLogs.OrderByDescending(log => log.OccurredAtUtc));
            
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"error_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }

        [HttpDelete("cleanup")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> CleanupOldLogs(CancellationToken cancellationToken, [FromQuery] int daysToKeep = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var errorLogs = await _errorLogDb.GetAllAsync(cancellationToken);
            var oldLogs = errorLogs.Where(log => log.OccurredAtUtc < cutoffDate);

            int deletedCount = 0;
            foreach (var log in oldLogs)
            {
                await _mediator.Send(new DeleteCommand<Domain.Entities.ErrorLog>(log.Id), cancellationToken);
                deletedCount++;
            }

            return Ok(new { DeletedCount = deletedCount, CutoffDate = cutoffDate });
        }

        private string GenerateCsv(IEnumerable<Domain.Entities.ErrorLog> logs)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Timestamp,ErrorType,StatusCode,Message,RequestPath,RequestMethod,UserId,StackTrace");
            
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.OccurredAtUtc:yyyy-MM-dd HH:mm:ss},{log.ErrorType},{log.StatusCode},\"{log.Message.Replace("\"", "\"\"")}\",{log.RequestPath},{log.RequestMethod},{log.UserId},\"{log.StackTrace?.Replace("\"", "\"\"")}\"");
            }
            
            return csv.ToString();
        }
    }

    public class ErrorLogFilterDto
    {
        public string? ErrorType { get; set; }
        public int? StatusCode { get; set; }
        public string? UserId { get; set; }
        public string? RequestPath { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class ErrorLogSummaryDto
    {
        public int TotalErrors { get; set; }
        public int ClientErrors { get; set; }
        public int ServerErrors { get; set; }
        public object TopErrorTypes { get; set; } = new object();
        public object TopStatusCodes { get; set; } = new object();
        public object TopEndpoints { get; set; } = new object();
        public object TopUsers { get; set; } = new object();
    }

    public class ErrorLogExportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ErrorType { get; set; }
        public int? StatusCode { get; set; }
    }
}
