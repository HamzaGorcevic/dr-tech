using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using drTech_backend.Infrastructure.Abstractions;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HospitalAdmin,Doctor")]
    public class EquipmentController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public EquipmentController(
			IMediator mediator,
			IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}

        [HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var equipment = await _mediator.Send(new GetAllQuery<Domain.Entities.Equipment>(), cancellationToken);
			var dtos = equipment.Select(e => _mapper.Map<EquipmentDto>(e)).ToList();
			return Ok(dtos);
		}

        [HttpGet("{id:guid}")]
		public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
		{
			var equipment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Equipment>(id), cancellationToken);
			return equipment is null ? NotFound() : Ok(_mapper.Map<EquipmentDto>(equipment));
		}

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] EquipmentCreateDto request, CancellationToken cancellationToken)
        {
            var equipment = _mapper.Map<Domain.Entities.Equipment>(request);
            equipment.Id = Guid.NewGuid();
            equipment.Status = "Operational";
            equipment.IsWithdrawn = false;

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

            var response = _mapper.Map<EquipmentDto>(equipment);
            return CreatedAtAction(nameof(Get), new { id = equipment.Id }, response);
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

            return Ok(_mapper.Map<EquipmentDto>(equipment));
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

            return Ok(_mapper.Map<EquipmentServiceOrderDto>(serviceOrder));
        }

        [HttpGet("{id:guid}/status-history")]
        public async Task<IActionResult> GetStatusHistory(Guid id, CancellationToken cancellationToken)
        {
			var logs = await _mediator.Send(new GetAllQuery<Domain.Entities.EquipmentStatusLog>(), cancellationToken);
            var equipmentLogs = logs.Where(log => log.EquipmentId == id).OrderByDescending(log => log.LoggedAtUtc);
            var dtos = equipmentLogs.Select(l => _mapper.Map<EquipmentStatusLogDto>(l)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id:guid}/service-orders")]
        public async Task<IActionResult> GetServiceOrders(Guid id, CancellationToken cancellationToken)
        {
			var orders = await _mediator.Send(new GetAllQuery<Domain.Entities.EquipmentServiceOrder>(), cancellationToken);
            var equipmentOrders = orders.Where(order => order.EquipmentId == id).OrderByDescending(order => order.ScheduledAtUtc);
            var dtos = equipmentOrders.Select(o => _mapper.Map<EquipmentServiceOrderDto>(o)).ToList();
            return Ok(dtos);
        }
    }

    // moved to Application.Common.DTOs

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
