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
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DoctorsController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Doctor>(), cancellationToken);
            var dtos = items.Select(d => _mapper.Map<DoctorDto>(d)).ToList();
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] DoctorCreateDto request, CancellationToken cancellationToken)
        {
            // Validate department exists to avoid FK violation
            var department = await _mediator.Send(new GetByIdQuery<Domain.Entities.Department>(request.DepartmentId), cancellationToken);
            if (department is null) return BadRequest("Invalid DepartmentId");

            // Validate user exists to satisfy FK to Users
            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(request.UserId), cancellationToken);
            if (user is null) return BadRequest("Invalid UserId");

            var entity = _mapper.Map<Domain.Entities.Doctor>(request);
            entity.Id = Guid.NewGuid();
            await _mediator.Send(new CreateCommand<Domain.Entities.Doctor>(entity), cancellationToken);
            var response = _mapper.Map<DoctorDto>(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, response);
        }
    }
}


