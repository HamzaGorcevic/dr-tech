using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HospitalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HospitalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Hospital>(), cancellationToken);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateHospitalDto request, CancellationToken cancellationToken)
        {
            var hospital = new Domain.Entities.Hospital
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                City = request.City,
                Departments = new List<Domain.Entities.Department>()
            };
            
            await _mediator.Send(new CreateCommand<Domain.Entities.Hospital>(hospital), cancellationToken);
            
            var response = new HospitalResponseDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                City = hospital.City,
                DepartmentsCount = 0
            };
            
            return CreatedAtAction(nameof(Get), new { id = hospital.Id }, response);
        }
    }
}


