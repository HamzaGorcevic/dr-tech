using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiscountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabaseService<Domain.Entities.Discount> _discountDb;
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _discountRequestDb;
        private readonly IDatabaseService<Domain.Entities.PriceListItem> _priceListDb;

        public DiscountController(
            IMediator mediator,
            IDatabaseService<Domain.Entities.Discount> discountDb,
            IDatabaseService<Domain.Entities.DiscountRequest> discountRequestDb,
            IDatabaseService<Domain.Entities.PriceListItem> priceListDb)
        {
            _mediator = mediator;
            _discountDb = discountDb;
            _discountRequestDb = discountRequestDb;
            _priceListDb = priceListDb;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var discounts = await _mediator.Send(new GetAllQuery<Domain.Entities.Discount>(), cancellationToken);
            return Ok(discounts);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var discount = await _mediator.Send(new GetByIdQuery<Domain.Entities.Discount>(id), cancellationToken);
            return discount is null ? NotFound() : Ok(discount);
        }

        [HttpPost("calculate")]
        [Authorize(Roles = "InsuranceAgency,HospitalAdmin")]
        public async Task<IActionResult> CalculateDiscount([FromBody] CalculateDiscountDto request, CancellationToken cancellationToken)
        {
            var priceList = await _priceListDb.GetAllAsync(cancellationToken);
            var totalValue = priceList
                .Where(p => request.ServiceIds.Contains(p.MedicalServiceId))
                .Sum(p => p.Price);

            decimal discountPercent = 0;
            decimal maxDiscountAmount = 0;

            // Calculate discount based on total value
            if (totalValue >= 10000)
            {
                discountPercent = 15;
                maxDiscountAmount = 2000;
            }
            else if (totalValue >= 5000)
            {
                discountPercent = 10;
                maxDiscountAmount = 1000;
            }
            else if (totalValue >= 1000)
            {
                discountPercent = 5;
                maxDiscountAmount = 200;
            }

            var result = new DiscountCalculationResult
            {
                TotalValue = totalValue,
                DiscountPercent = discountPercent,
                MaxDiscountAmount = maxDiscountAmount,
                CalculatedDiscount = Math.Min(totalValue * discountPercent / 100, maxDiscountAmount)
            };

            return Ok(result);
        }

        [HttpPost("request")]
        [Authorize(Roles = "InsuranceAgency")]
        public async Task<IActionResult> RequestDiscount([FromBody] CreateDiscountRequestDto request, CancellationToken cancellationToken)
        {
            var discountRequest = new Domain.Entities.DiscountRequest
            {
                Id = Guid.NewGuid(),
                InsuranceAgencyId = request.InsuranceAgencyId,
                HospitalId = request.HospitalId,
                PatientId = request.PatientId,
                RequestedDiscountPercent = request.RequestedDiscountPercent,
                Reason = request.Reason,
                Explanation = request.Explanation,
                Status = "Pending",
                RequestedAtUtc = DateTime.UtcNow
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.DiscountRequest>(discountRequest), cancellationToken);
            return CreatedAtAction(nameof(GetDiscountRequest), new { id = discountRequest.Id }, discountRequest);
        }

        [HttpGet("requests")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency")]
        public async Task<IActionResult> GetDiscountRequests(CancellationToken cancellationToken)
        {
            var requests = await _mediator.Send(new GetAllQuery<Domain.Entities.DiscountRequest>(), cancellationToken);
            return Ok(requests);
        }

        [HttpGet("requests/{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency")]
        public async Task<IActionResult> GetDiscountRequest(Guid id, CancellationToken cancellationToken)
        {
            var request = await _mediator.Send(new GetByIdQuery<Domain.Entities.DiscountRequest>(id), cancellationToken);
            return request is null ? NotFound() : Ok(request);
        }

        [HttpPut("requests/{id:guid}/approve")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> ApproveDiscountRequest(Guid id, [FromBody] ApproveDiscountRequestDto request, CancellationToken cancellationToken)
        {
            var discountRequest = await _mediator.Send(new GetByIdQuery<Domain.Entities.DiscountRequest>(id), cancellationToken);
            if (discountRequest is null) return NotFound();

            discountRequest.Status = "Approved";
            discountRequest.RespondedAtUtc = DateTime.UtcNow;

            await _mediator.Send(new UpdateCommand<Domain.Entities.DiscountRequest>(discountRequest), cancellationToken);

            // Create the actual discount
            var discount = new Domain.Entities.Discount
            {
                Id = Guid.NewGuid(),
                PatientId = discountRequest.PatientId,
                HospitalId = discountRequest.HospitalId,
                InsuranceAgencyId = discountRequest.InsuranceAgencyId,
                DiscountPercent = request.ApprovedDiscountPercent,
                MaxDiscountAmount = request.MaxDiscountAmount,
                Reason = discountRequest.Reason,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddMonths(6),
                IsActive = true,
                Status = "Approved"
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.Discount>(discount), cancellationToken);

            return Ok(new { discountRequest, discount });
        }

        [HttpPut("requests/{id:guid}/reject")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> RejectDiscountRequest(Guid id, [FromBody] RejectDiscountRequestDto request, CancellationToken cancellationToken)
        {
            var discountRequest = await _mediator.Send(new GetByIdQuery<Domain.Entities.DiscountRequest>(id), cancellationToken);
            if (discountRequest is null) return NotFound();

            discountRequest.Status = "Rejected";
            discountRequest.RejectionReason = request.RejectionReason;
            discountRequest.RespondedAtUtc = DateTime.UtcNow;

            await _mediator.Send(new UpdateCommand<Domain.Entities.DiscountRequest>(discountRequest), cancellationToken);
            return Ok(discountRequest);
        }

        [HttpGet("patient/{patientId:guid}")]
        [Authorize(Roles = "HospitalAdmin,Doctor,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> GetPatientDiscounts(Guid patientId, CancellationToken cancellationToken)
        {
            var discounts = await _discountDb.GetAllAsync(cancellationToken);
            var patientDiscounts = discounts
                .Where(d => d.PatientId == patientId && d.IsActive && d.ValidUntil > DateTime.UtcNow)
                .OrderByDescending(d => d.ValidFrom);
            return Ok(patientDiscounts);
        }

        [HttpPut("{id:guid}/deactivate")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> DeactivateDiscount(Guid id, CancellationToken cancellationToken)
        {
            var discount = await _mediator.Send(new GetByIdQuery<Domain.Entities.Discount>(id), cancellationToken);
            if (discount is null) return NotFound();

            discount.IsActive = false;
            await _mediator.Send(new UpdateCommand<Domain.Entities.Discount>(discount), cancellationToken);
            return Ok(discount);
        }
    }

    public class CalculateDiscountDto
    {
        public List<Guid> ServiceIds { get; set; } = new List<Guid>();
    }

    public class DiscountCalculationResult
    {
        public decimal TotalValue { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal CalculatedDiscount { get; set; }
    }

    public class CreateDiscountRequestDto
    {
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public Guid PatientId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    public class ApproveDiscountRequestDto
    {
        public decimal ApprovedDiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
    }

    public class RejectDiscountRequestDto
    {
        public string RejectionReason { get; set; } = string.Empty;
    }
}
