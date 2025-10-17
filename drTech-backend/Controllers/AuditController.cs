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
    public class AuditController : ControllerBase
    {
		private readonly IMediator _mediator;

		public AuditController(
			IMediator mediator)
		{
			_mediator = mediator;
		}

        [HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] AuditFilterDto filter, CancellationToken cancellationToken)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            
            // Apply filters
            if (!string.IsNullOrEmpty(filter.Actor))
                auditLogs = auditLogs.Where(log => log.Actor.Contains(filter.Actor, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(filter.Action))
                auditLogs = auditLogs.Where(log => log.Action == filter.Action).ToList();
            
            if (!string.IsNullOrEmpty(filter.Path))
                auditLogs = auditLogs.Where(log => log.Path.Contains(filter.Path, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (filter.StatusCode.HasValue)
                auditLogs = auditLogs.Where(log => log.StatusCode == filter.StatusCode.Value).ToList();
            
            if (filter.StartDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc >= filter.StartDate.Value).ToList();
            
            if (filter.EndDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc <= filter.EndDate.Value).ToList();

            // Order by most recent first
            auditLogs = auditLogs.OrderByDescending(log => log.OccurredAtUtc).ToList();

            // Apply pagination
            if (filter.PageSize > 0)
            {
                var skip = (filter.Page - 1) * filter.PageSize;
                auditLogs = auditLogs.Skip(skip).Take(filter.PageSize).ToList();
            }

            return Ok(auditLogs);
        }

        [HttpGet("{id:guid}")]
		public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
			var auditLog = await _mediator.Send(new GetByIdQuery<Domain.Entities.AuditLog>(id), cancellationToken);
            return auditLog is null ? NotFound() : Ok(auditLog);
        }

        [HttpGet("actor/{actor}")]
		public async Task<IActionResult> GetByActor(string actor, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            var actorLogs = auditLogs
                .Where(log => log.Actor.Contains(actor, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(actorLogs);
        }

        [HttpGet("action/{action}")]
		public async Task<IActionResult> GetByAction(string action, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            var actionLogs = auditLogs
                .Where(log => log.Action == action)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(actionLogs);
        }

        [HttpGet("hospital/{hospitalId:guid}")]
        [Authorize(Roles = "HospitalAdmin")]
		public async Task<IActionResult> GetByHospital(Guid hospitalId, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            var hospitalLogs = auditLogs
                .Where(log => log.Path.Contains($"hospital/{hospitalId}", StringComparison.OrdinalIgnoreCase) ||
                             log.Description?.Contains($"HospitalId:{hospitalId}") == true)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(hospitalLogs);
        }

        [HttpGet("agency/{agencyId:guid}")]
        [Authorize(Roles = "InsuranceAgency")]
		public async Task<IActionResult> GetByAgency(Guid agencyId, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            var agencyLogs = auditLogs
                .Where(log => log.Path.Contains($"agency/{agencyId}", StringComparison.OrdinalIgnoreCase) ||
                             log.Description?.Contains($"AgencyId:{agencyId}") == true)
                .OrderByDescending(log => log.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(agencyLogs);
        }

        [HttpGet("summary")]
		public async Task<IActionResult> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            
            if (startDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc >= startDate.Value).ToList();
            
            if (endDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc <= endDate.Value).ToList();

            var summary = new AuditSummaryDto
            {
                TotalActions = auditLogs.Count(),
                CreateActions = auditLogs.Count(log => log.Action == "Create"),
                UpdateActions = auditLogs.Count(log => log.Action == "Update"),
                DeleteActions = auditLogs.Count(log => log.Action == "Delete"),
                SuccessfulActions = auditLogs.Count(log => log.StatusCode >= 200 && log.StatusCode < 300),
                FailedActions = auditLogs.Count(log => log.StatusCode >= 400),
                TopActors = auditLogs
                    .GroupBy(log => log.Actor)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { Actor = g.Key, Count = g.Count() }),
                TopActions = auditLogs
                    .GroupBy(log => log.Action)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { Action = g.Key, Count = g.Count() }),
                TopEndpoints = auditLogs
                    .GroupBy(log => log.Path)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { Endpoint = g.Key, Count = g.Count() })
            };

            return Ok(summary);
        }

        [HttpPost("export")]
		public async Task<IActionResult> ExportAuditLogs([FromBody] AuditExportDto request, CancellationToken cancellationToken)
        {
			var auditLogs = await _mediator.Send(new GetAllQuery<Domain.Entities.AuditLog>(), cancellationToken);
            
            // Apply filters
            if (request.StartDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc >= request.StartDate.Value).ToList();
            
            if (request.EndDate.HasValue)
                auditLogs = auditLogs.Where(log => log.OccurredAtUtc <= request.EndDate.Value).ToList();
            
            if (!string.IsNullOrEmpty(request.Actor))
                auditLogs = auditLogs.Where(log => log.Actor.Contains(request.Actor, StringComparison.OrdinalIgnoreCase)).ToList();

            var csv = GenerateCsv(auditLogs.OrderByDescending(log => log.OccurredAtUtc));
            
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"audit_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }

        private string GenerateCsv(IEnumerable<Domain.Entities.AuditLog> logs)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Timestamp,Actor,Action,Path,Method,StatusCode,Description");
            
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.OccurredAtUtc:yyyy-MM-dd HH:mm:ss},{log.Actor},{log.Action},{log.Path},{log.Method},{log.StatusCode},\"{log.Description?.Replace("\"", "\"\"")}\"");
            }
            
            return csv.ToString();
        }
    }

    public class AuditFilterDto
    {
        public string? Actor { get; set; }
        public string? Action { get; set; }
        public string? Path { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class AuditSummaryDto
    {
        public int TotalActions { get; set; }
        public int CreateActions { get; set; }
        public int UpdateActions { get; set; }
        public int DeleteActions { get; set; }
        public int SuccessfulActions { get; set; }
        public int FailedActions { get; set; }
        public object TopActors { get; set; } = new object();
        public object TopActions { get; set; } = new object();
        public object TopEndpoints { get; set; } = new object();
    }

    public class AuditExportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Actor { get; set; }
    }
}
