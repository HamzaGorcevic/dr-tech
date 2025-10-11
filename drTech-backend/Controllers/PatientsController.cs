using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public PatientsController(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _db.GetAllAsync(cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var item = await _db.GetByIdAsync(id, cancellationToken);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.Patient request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _db.AddAsync(request, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = request.Id }, request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Domain.Entities.Patient request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest();
            await _db.UpdateAsync(request, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(id, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}


