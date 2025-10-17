using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public ReservationsController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuredUser")]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Reservation>(), cancellationToken);
			var dtos = items.Select(r => _mapper.Map<ReservationDto>(r)).ToList();
			return Ok(dtos);
		}

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuredUser")]
		public async Task<IActionResult> Create([FromBody] ReservationCreateDto request, CancellationToken cancellationToken)
        {
            // enforce one active reservation per patient overlapping period
			var existingReservations = await _mediator.Send(new GetAllQuery<Domain.Entities.Reservation>(), cancellationToken);
            var overlap = existingReservations.Any(r => r.PatientId == request.PatientId && r.Status != "Cancelled" &&
                r.StartsAtUtc < request.EndsAtUtc && request.StartsAtUtc < r.EndsAtUtc);
            if (overlap) return Conflict("Patient already has an active reservation in this period.");

            var entity = _mapper.Map<Domain.Entities.Reservation>(request);
            entity.Id = Guid.NewGuid();
			await _mediator.Send(new CreateCommand<Domain.Entities.Reservation>(entity), cancellationToken);
            var response = _mapper.Map<ReservationDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


