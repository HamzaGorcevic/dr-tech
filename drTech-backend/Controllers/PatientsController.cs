using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PatientsController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.Patient>(), cancellationToken));

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var item = await _mediator.Send(new GetByIdQuery<Domain.Entities.Patient>(id), cancellationToken);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency")]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.Patient request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.Patient>(request), cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = request.Id }, request);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Domain.Entities.Patient request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest();
            await _mediator.Send(new UpdateCommand<Domain.Entities.Patient>(request), cancellationToken);
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


