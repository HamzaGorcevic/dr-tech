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
    public class ServicesController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public ServicesController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [AllowAnonymous]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var items = await _mediator.Send(new GetAllQuery<Domain.Entities.MedicalService>(), cancellationToken);
			var dtos = items.Select(s => _mapper.Map<MedicalServiceDto>(s)).ToList();
			return Ok(dtos);
		}

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
		public async Task<IActionResult> Create([FromBody] MedicalServiceCreateDto request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.MedicalService>(request);
            entity.Id = Guid.NewGuid();
			await _mediator.Send(new CreateCommand<Domain.Entities.MedicalService>(entity), cancellationToken);
            var response = _mapper.Map<MedicalServiceDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


