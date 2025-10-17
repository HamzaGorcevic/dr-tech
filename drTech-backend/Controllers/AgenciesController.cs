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
    public class AgenciesController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public AgenciesController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [AllowAnonymous]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.InsuranceAgency>(), cancellationToken));

        [HttpPost]
        [Authorize(Roles = "InsuranceAgency")]
		public async Task<IActionResult> Create([FromBody] InsuranceAgencyCreateDto request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.InsuranceAgency>(request);
            entity.Id = Guid.NewGuid();
			await _mediator.Send(new CreateCommand<Domain.Entities.InsuranceAgency>(entity), cancellationToken);
            var response = _mapper.Map<InsuranceAgencyDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


