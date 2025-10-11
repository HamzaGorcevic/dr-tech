using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.Payment> _db;
        public PaymentsController(IDatabaseService<Domain.Entities.Payment> db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _db.GetAllAsync(cancellationToken));

        public class PaymentUploadDto
        {
            public Guid PreContractId { get; set; }
            public decimal Amount { get; set; }
            public DateTime DueDateUtc { get; set; }
            public IFormFile? Proof { get; set; }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Create([FromForm] PaymentUploadDto request, CancellationToken cancellationToken)
        {
            var payment = new Domain.Entities.Payment
            {
                Id = Guid.NewGuid(),
                PreContractId = request.PreContractId,
                Amount = request.Amount,
                DueDateUtc = request.DueDateUtc,
                Confirmed = false,
                LateCount = 0
            };

            if (request.Proof != null && request.Proof.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(uploads);
                var path = Path.Combine(uploads, payment.Id + Path.GetExtension(request.Proof.FileName));
                using var fs = new FileStream(path, FileMode.Create);
                await request.Proof.CopyToAsync(fs, cancellationToken);
                payment.ProofUrl = path;
            }

            await _db.AddAsync(payment, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(payment);
        }
    }
}


