using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgenciesController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public AgenciesController(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _db.GetAllAsync(cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.InsuranceAgency request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _db.AddAsync(request, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }
    }
}


