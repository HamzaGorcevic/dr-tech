using Microsoft.AspNetCore.Mvc;
using MediatR;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ServicesController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.MedicalService>(), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.MedicalService request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.MedicalService>(request), cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }
    }
}


