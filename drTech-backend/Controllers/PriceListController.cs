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
    public class PriceListController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public PriceListController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.PriceListItem>(), cancellationToken);
            var dtos = items.Select(p => _mapper.Map<PriceListItemDto>(p)).ToList();
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] PriceListItemCreateDto request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.PriceListItem>(request);
            entity.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.PriceListItem>(entity), cancellationToken);
            var response = _mapper.Map<PriceListItemDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


