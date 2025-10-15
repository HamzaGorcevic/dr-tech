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
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DepartmentsController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Department>(), cancellationToken);
            var dtos = items.Select(d => _mapper.Map<DepartmentDto>(d)).ToList();
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateDto request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.Department>(request);
            entity.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.Department>(entity), cancellationToken);
            var response = _mapper.Map<DepartmentDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


