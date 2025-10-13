using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.Department> _db;
        public DepartmentsController(IDatabaseService<Domain.Entities.Department> db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _db.GetAllAsync(cancellationToken));

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
            
            await _db.AddAsync(department, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            
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


