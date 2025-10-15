using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using drTech_backend.Application.Common.Mediator;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HospitalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public HospitalsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Hospital>(), cancellationToken);
            var dtos = items.Select(h => _mapper.Map<HospitalDto>(h)).ToList();
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] HospitalCreateDto request, CancellationToken cancellationToken)
        {
            var hospital = _mapper.Map<Domain.Entities.Hospital>(request);
            hospital.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.Hospital>(hospital), cancellationToken);
            var response = _mapper.Map<HospitalDto>(hospital);
            return CreatedAtAction(nameof(Get), new { id = hospital.Id }, response);
        }
    }
}


