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
    public class PatientsController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public PatientsController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency")]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Patient>(), cancellationToken);
			var dtos = items.Select(p => _mapper.Map<PatientDto>(p)).ToList();
			return Ok(dtos);
		}

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency,InsuredUser")]
		public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
		{
			var item = await _mediator.Send(new GetByIdQuery<Domain.Entities.Patient>(id), cancellationToken);
			return item is null ? NotFound() : Ok(_mapper.Map<PatientDto>(item));
		}

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency")]
		public async Task<IActionResult> Create([FromBody] PatientCreateDto request, CancellationToken cancellationToken)
        {
            // Validate related FKs before creation
            if (request.InsuranceAgencyId.HasValue)
            {
				var agency = await _mediator.Send(new GetByIdQuery<Domain.Entities.InsuranceAgency>(request.InsuranceAgencyId.Value), cancellationToken);
                if (agency is null) return BadRequest("Invalid InsuranceAgencyId");
            }

			var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(request.UserId), cancellationToken);
            if (user is null) return BadRequest("Invalid UserId");

            var entity = _mapper.Map<Domain.Entities.Patient>(request);
            entity.Id = Guid.NewGuid();
			await _mediator.Send(new CreateCommand<Domain.Entities.Patient>(entity), cancellationToken);
            var response = _mapper.Map<PatientDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency,InsuredUser")]
		public async Task<IActionResult> Update(Guid id, [FromBody] PatientDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest();
            var entity = _mapper.Map<Domain.Entities.Patient>(request);
			await _mediator.Send(new UpdateCommand<Domain.Entities.Patient>(entity), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
			await _mediator.Send(new DeleteCommand<Domain.Entities.Patient>(id), cancellationToken);
            return NoContent();
        }
    }
}


