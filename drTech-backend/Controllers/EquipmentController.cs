using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HospitalAdmin,Doctor")]
    public class EquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabaseService<Domain.Entities.Equipment> _equipmentDb;
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _statusLogDb;
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _serviceOrderDb;

        public EquipmentController(
            IMediator mediator,
            IDatabaseService<Domain.Entities.Equipment> equipmentDb,
            IDatabaseService<Domain.Entities.EquipmentStatusLog> statusLogDb,
            IDatabaseService<Domain.Entities.EquipmentServiceOrder> serviceOrderDb)
        {
            _mediator = mediator;
            _equipmentDb = equipmentDb;
            _statusLogDb = statusLogDb;
            _serviceOrderDb = serviceOrderDb;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var equipment = await _mediator.Send(new GetAllQuery<Domain.Entities.Equipment>(), cancellationToken);
            return Ok(equipment);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var equipment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Equipment>(id), cancellationToken);
            return equipment is null ? NotFound() : Ok(equipment);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentDto request, CancellationToken cancellationToken)
        {
            var equipment = new Domain.Entities.Equipment
            {
                Id = Guid.NewGuid(),
                SerialNumber = request.SerialNumber,
                Type = request.Type,
                Status = "Operational",
                DepartmentId = request.DepartmentId,
                IsWithdrawn = false
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.Equipment>(equipment), cancellationToken);

            // Log initial status
            var statusLog = new Domain.Entities.EquipmentStatusLog
            {
                Id = Guid.NewGuid(),
                EquipmentId = equipment.Id,
                Status = "Operational",
                Note = "Equipment registered",
                LoggedAtUtc = DateTime.UtcNow
            };
            await _mediator.Send(new CreateCommand<Domain.Entities.EquipmentStatusLog>(statusLog), cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = equipment.Id }, equipment);
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "HospitalAdmin,Doctor")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateEquipmentStatusDto request, CancellationToken cancellationToken)
        {
            var equipment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Equipment>(id), cancellationToken);
            if (equipment is null) return NotFound();

            equipment.Status = request.Status;
            if (request.Status == "Withdrawn") equipment.IsWithdrawn = true;

            await _mediator.Send(new UpdateCommand<Domain.Entities.Equipment>(equipment), cancellationToken);

            // Log status change
            var statusLog = new Domain.Entities.EquipmentStatusLog
            {
                Id = Guid.NewGuid(),
                EquipmentId = equipment.Id,
                Status = request.Status,
                Note = request.Note,
                LoggedAtUtc = DateTime.UtcNow
            };
            await _mediator.Send(new CreateCommand<Domain.Entities.EquipmentStatusLog>(statusLog), cancellationToken);

            return Ok(equipment);
        }

        [HttpPost("{id:guid}/schedule-service")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> ScheduleService(Guid id, [FromBody] ScheduleServiceDto request, CancellationToken cancellationToken)
        {
            var equipment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Equipment>(id), cancellationToken);
            if (equipment is null) return NotFound();

            var serviceOrder = new Domain.Entities.EquipmentServiceOrder
            {
                Id = Guid.NewGuid(),
                EquipmentId = id,
                Type = request.Type,
                ScheduledAtUtc = request.ScheduledAtUtc,
                Status = "Scheduled"
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.EquipmentServiceOrder>(serviceOrder), cancellationToken);

            return Ok(serviceOrder);
        }

        [HttpGet("{id:guid}/status-history")]
        public async Task<IActionResult> GetStatusHistory(Guid id, CancellationToken cancellationToken)
        {
            var logs = await _statusLogDb.GetAllAsync(cancellationToken);
            var equipmentLogs = logs.Where(log => log.EquipmentId == id).OrderByDescending(log => log.LoggedAtUtc);
            return Ok(equipmentLogs);
        }

        [HttpGet("{id:guid}/service-orders")]
        public async Task<IActionResult> GetServiceOrders(Guid id, CancellationToken cancellationToken)
        {
            var orders = await _serviceOrderDb.GetAllAsync(cancellationToken);
            var equipmentOrders = orders.Where(order => order.EquipmentId == id).OrderByDescending(order => order.ScheduledAtUtc);
            return Ok(equipmentOrders);
        }
    }

    public class CreateEquipmentDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
    }

    public class UpdateEquipmentStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class ScheduleServiceDto
    {
        public string Type { get; set; } = "Service";
        public DateTime ScheduledAtUtc { get; set; }
    }
}
