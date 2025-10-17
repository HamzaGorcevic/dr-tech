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
    public class PaymentsController : ControllerBase
    {
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public PaymentsController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        [HttpGet]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency,InsuredUser")]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var items = await _mediator.Send(new GetAllQuery<Domain.Entities.Payment>(), cancellationToken);
			var dtos = items.Select(p => _mapper.Map<PaymentDto>(p)).ToList();
			return Ok(dtos);
		}

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
        [Authorize(Roles = "InsuredUser")]
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

			await _mediator.Send(new CreateCommand<Domain.Entities.Payment>(payment), cancellationToken);
            var response = _mapper.Map<PaymentDto>(payment);
            return Ok(response);
        }
    }
}


