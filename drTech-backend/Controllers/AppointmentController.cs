using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public AppointmentController(
			IMediator mediator,
			IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}

        [HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var appointments = await _mediator.Send(new GetAllQuery<Domain.Entities.Appointment>(), cancellationToken);
			var dtos = appointments.Select(a => _mapper.Map<AppointmentDto>(a)).ToList();
			return Ok(dtos);
		}

        [HttpGet("{id:guid}")]
		public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
		{
			var appointment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Appointment>(id), cancellationToken);
			return appointment is null ? NotFound() : Ok(_mapper.Map<AppointmentDto>(appointment));
		}

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin,Doctor")]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto request, CancellationToken cancellationToken)
        {
            // Check doctor availability
			var doctor = await _mediator.Send(new GetByIdQuery<Domain.Entities.Doctor>(request.DoctorId), cancellationToken);
            if (doctor is null || !doctor.IsAvailable) return BadRequest("Doctor not available");

            // Check equipment availability
            if (request.RequiredEquipmentIds?.Any() == true)
            {
				var equipment = await _mediator.Send(new GetAllQuery<Domain.Entities.Equipment>(), cancellationToken);
                var requiredEquipment = equipment.Where(e => request.RequiredEquipmentIds.Contains(e.Id));
                if (requiredEquipment.Any(e => e.Status != "Operational" || e.IsWithdrawn))
                    return BadRequest("Required equipment not available");
            }

            // Check for conflicts
			var existingAppointments = await _mediator.Send(new GetAllQuery<Domain.Entities.Appointment>(), cancellationToken);
            var doctorConflict = existingAppointments.Any(a => 
                a.DoctorId == request.DoctorId && 
                a.Status != "Cancelled" &&
                a.StartsAtUtc < request.EndsAtUtc && 
                request.StartsAtUtc < a.EndsAtUtc);

            if (doctorConflict) return Conflict("Doctor has conflicting appointment");

            var appointment = new Domain.Entities.Appointment
            {
                Id = Guid.NewGuid(),
                HospitalId = request.HospitalId,
                DepartmentId = request.DepartmentId,
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                MedicalServiceId = request.MedicalServiceId,
                StartsAtUtc = request.StartsAtUtc,
                EndsAtUtc = request.EndsAtUtc,
                Type = request.Type,
                Status = "Scheduled",
                IsConfirmed = false,
                RequiredEquipmentIds = request.RequiredEquipmentIds ?? new List<Guid>(),
                Notes = request.Notes
            };

			await _mediator.Send(new CreateCommand<Domain.Entities.Appointment>(appointment), cancellationToken);
            var response = _mapper.Map<AppointmentDto>(appointment);
            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, response);
        }

        [HttpPut("{id:guid}/reschedule")]
        [Authorize(Roles = "HospitalAdmin,Doctor")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentDto request, CancellationToken cancellationToken)
        {
			var appointment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Appointment>(id), cancellationToken);
            if (appointment is null) return NotFound();

            appointment.StartsAtUtc = request.NewStartsAtUtc;
            appointment.EndsAtUtc = request.NewEndsAtUtc;
            appointment.RescheduleCount++;

            // Check if this is the third reschedule by institution
            if (appointment.RescheduleCount >= 2)
            {
                // Create automatic discount
                var discount = new Domain.Entities.Discount
                {
                    Id = Guid.NewGuid(),
                    PatientId = appointment.PatientId,
                    HospitalId = appointment.HospitalId,
                    DiscountPercent = 10, // 10% discount for rescheduling
                    MaxDiscountAmount = 100, // Max $100 discount
                    Reason = "Reschedule",
                    ValidFrom = DateTime.UtcNow,
                    ValidUntil = DateTime.UtcNow.AddMonths(6),
                    IsActive = true,
                    Status = "Approved"
                };
				await _mediator.Send(new CreateCommand<Domain.Entities.Discount>(discount), cancellationToken);
            }

			await _mediator.Send(new UpdateCommand<Domain.Entities.Appointment>(appointment), cancellationToken);
            return Ok(_mapper.Map<AppointmentDto>(appointment));
        }

        [HttpPut("{id:guid}/confirm")]
        [Authorize(Roles = "HospitalAdmin,Doctor")]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
        {
			var appointment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Appointment>(id), cancellationToken);
            if (appointment is null) return NotFound();

            appointment.IsConfirmed = true;
            appointment.Status = "Confirmed";

			await _mediator.Send(new UpdateCommand<Domain.Entities.Appointment>(appointment), cancellationToken);
            return Ok(_mapper.Map<AppointmentDto>(appointment));
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuredUser")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentDto request, CancellationToken cancellationToken)
        {
			var appointment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Appointment>(id), cancellationToken);
            if (appointment is null) return NotFound();

            appointment.Status = "Cancelled";
            appointment.Notes = request.Reason;

			await _mediator.Send(new UpdateCommand<Domain.Entities.Appointment>(appointment), cancellationToken);
            return Ok(appointment);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor")]
        public async Task<IActionResult> GetByDoctor(Guid doctorId, CancellationToken cancellationToken)
        {
			var appointments = await _mediator.Send(new GetAllQuery<Domain.Entities.Appointment>(), cancellationToken);
            var doctorAppointments = appointments.Where(a => a.DoctorId == doctorId).OrderBy(a => a.StartsAtUtc);
            return Ok(doctorAppointments);
        }

        [HttpGet("patient/{patientId:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuredUser")]
        public async Task<IActionResult> GetByPatient(Guid patientId, CancellationToken cancellationToken)
        {
			var appointments = await _mediator.Send(new GetAllQuery<Domain.Entities.Appointment>(), cancellationToken);
            var patientAppointments = appointments.Where(a => a.PatientId == patientId).OrderBy(a => a.StartsAtUtc);
            return Ok(patientAppointments);
        }
    }

    // moved to Application.Common.DTOs

    public class RescheduleAppointmentDto
    {
        public DateTime NewStartsAtUtc { get; set; }
        public DateTime NewEndsAtUtc { get; set; }
    }

    public class CancelAppointmentDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
