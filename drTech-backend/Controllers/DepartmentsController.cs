using Microsoft.AspNetCore.Mvc;
using MediatR;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentsController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllQuery<Domain.Entities.Department>(), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto request, CancellationToken cancellationToken)
        {
            var department = new Domain.Entities.Department
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DoctorsCount = request.DoctorsCount,
                HospitalId = request.HospitalId
            };
            
            await _mediator.Send(new CreateCommand<Domain.Entities.Department>(department), cancellationToken);
            
            var response = new DepartmentResponseDto
            {
                Id = department.Id,
                Name = department.Name,
                DoctorsCount = department.DoctorsCount,
                HospitalId = department.HospitalId,
                HospitalName = "Unknown"
            };
            
            return CreatedAtAction(nameof(GetAll), new { id = department.Id }, response);
        }
    }
}


