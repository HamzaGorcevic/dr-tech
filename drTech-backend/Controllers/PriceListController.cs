using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PriceListController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PriceListController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.PriceListItem>(), cancellationToken));

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.PriceListItem request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.PriceListItem>(request), cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }
    }
}


