using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.Reservation> _db;
        public ReservationsController(IDatabaseService<Domain.Entities.Reservation> db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _db.GetAllAsync(cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Entities.Reservation request, CancellationToken cancellationToken)
        {
            // enforce one active reservation per patient overlapping period
            var existingReservations = await _db.GetAllAsync(cancellationToken);
            var overlap = existingReservations.Any(r => r.PatientId == request.PatientId && r.Status != "Cancelled" &&
                r.StartsAtUtc < request.EndsAtUtc && request.StartsAtUtc < r.EndsAtUtc);
            if (overlap) return Conflict("Patient already has an active reservation in this period.");

            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            await _db.AddAsync(request, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }
    }
}


