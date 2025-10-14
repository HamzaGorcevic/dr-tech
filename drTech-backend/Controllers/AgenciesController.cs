using Microsoft.AspNetCore.Mvc;
using MediatR;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgenciesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AgenciesController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.InsuranceAgency>(), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.InsuranceAgency request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.InsuranceAgency>(request), cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }
    }
}


