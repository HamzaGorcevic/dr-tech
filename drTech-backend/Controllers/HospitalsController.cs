using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;

        public HospitalsController(IDatabaseService<Domain.Entities.Hospital> db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHospitalDto request, CancellationToken cancellationToken)
        {
            var hospital = new Domain.Entities.Hospital
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                City = request.City,
                Departments = new List<Domain.Entities.Department>()
            };
            
            await _db.AddAsync(hospital, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            
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


