using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PreContractsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IDatabaseService<Domain.Entities.PreContract> _preContractDb;
        private readonly IDatabaseService<Domain.Entities.Payment> _paymentDb;
        private readonly IDatabaseService<Domain.Entities.Patient> _patientDb;
        private readonly IDatabaseService<Domain.Entities.Hospital> _hospitalDb;
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _agencyDb;
        private readonly IDatabaseService<Domain.Entities.PriceListItem> _priceListDb;

        public PreContractsController(
            IMediator mediator,
            IMapper mapper,
            IDatabaseService<Domain.Entities.PreContract> preContractDb,
            IDatabaseService<Domain.Entities.Payment> paymentDb,
            IDatabaseService<Domain.Entities.Patient> patientDb,
            IDatabaseService<Domain.Entities.Hospital> hospitalDb,
            IDatabaseService<Domain.Entities.InsuranceAgency> agencyDb,
            IDatabaseService<Domain.Entities.PriceListItem> priceListDb)
        {
            _mediator = mediator;
            _mapper = mapper;
            _preContractDb = preContractDb;
            _paymentDb = paymentDb;
            _patientDb = patientDb;
            _hospitalDb = hospitalDb;
            _agencyDb = agencyDb;
            _priceListDb = priceListDb;
        }

        [HttpGet]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetAllQuery<Domain.Entities.PreContract>(), cancellationToken);
            var dtos = items.Select(p => _mapper.Map<PreContractDto>(p)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var item = await _mediator.Send(new GetByIdQuery<Domain.Entities.PreContract>(id), cancellationToken);
            if (item is null) return NotFound();
            var dto = _mapper.Map<PreContractDto>(item);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> Create([FromBody] CreatePreContractDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null || !user.PatientId.HasValue) 
                return BadRequest("User not associated with patient");

            // Validate hospital exists
            var hospital = await _mediator.Send(new GetByIdQuery<Domain.Entities.Hospital>(request.HospitalId), cancellationToken);
            if (hospital is null) return BadRequest("Hospital not found");

            // Validate agency exists
            var agency = await _mediator.Send(new GetByIdQuery<Domain.Entities.InsuranceAgency>(request.InsuranceAgencyId), cancellationToken);
            if (agency is null) return BadRequest("Insurance agency not found");

            // Check if user has active pre-contract
            var existingContracts = await _preContractDb.GetAllAsync(cancellationToken);
            var activeContract = existingContracts.FirstOrDefault(pc => 
                pc.PatientId == user.PatientId.Value && 
                pc.Status == "Active");
            
            if (activeContract != null)
                return Conflict("Patient already has an active pre-contract");

            // Generate payment plan
            var paymentPlan = GeneratePaymentPlan(request.AgreedPrice, request.PaymentPlanType);
            
            var preContract = new Domain.Entities.PreContract
            {
                Id = Guid.NewGuid(),
                HospitalId = request.HospitalId,
                InsuranceAgencyId = request.InsuranceAgencyId,
                PatientId = user.PatientId.Value,
                AgreedPrice = request.AgreedPrice,
                PaymentPlan = paymentPlan,
                CreatedAtUtc = DateTime.UtcNow,
                Status = "Active"
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.PreContract>(preContract), cancellationToken);

            // Create payment installments
            await CreatePaymentInstallments(preContract, cancellationToken);

            var response = _mapper.Map<PreContractDto>(preContract);
            return CreatedAtAction(nameof(Get), new { id = preContract.Id }, response);
        }

        [HttpPut("{id:guid}/terminate")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency")]
        public async Task<IActionResult> Terminate(Guid id, [FromBody] TerminatePreContractDto request, CancellationToken cancellationToken)
        {
            var preContract = await _mediator.Send(new GetByIdQuery<Domain.Entities.PreContract>(id), cancellationToken);
            if (preContract is null) return NotFound();

            if (preContract.Status != "Active")
                return BadRequest("Only active pre-contracts can be terminated");

            preContract.Status = "Terminated";
            await _mediator.Send(new UpdateCommand<Domain.Entities.PreContract>(preContract), cancellationToken);

            // Process refunds based on payment status
            await ProcessRefunds(preContract, request.RefundPercentage, cancellationToken);

            return Ok(_mapper.Map<PreContractDto>(preContract));
        }

        [HttpPut("{id:guid}/complete")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            var preContract = await _mediator.Send(new GetByIdQuery<Domain.Entities.PreContract>(id), cancellationToken);
            if (preContract is null) return NotFound();

            if (preContract.Status != "Active")
                return BadRequest("Only active pre-contracts can be completed");

            // Check if all payments are confirmed
            var payments = await _paymentDb.GetAllAsync(cancellationToken);
            var contractPayments = payments.Where(p => p.PreContractId == id);
            
            if (!contractPayments.All(p => p.Confirmed))
                return BadRequest("All payments must be confirmed before completing contract");

            preContract.Status = "Completed";
            await _mediator.Send(new UpdateCommand<Domain.Entities.PreContract>(preContract), cancellationToken);

            return Ok(_mapper.Map<PreContractDto>(preContract));
        }

        [HttpGet("{id:guid}/payments")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency,InsuredUser")]
        public async Task<IActionResult> GetPayments(Guid id, CancellationToken cancellationToken)
        {
            var payments = await _paymentDb.GetAllAsync(cancellationToken);
            var contractPayments = payments.Where(p => p.PreContractId == id);
            var dtos = contractPayments.Select(p => _mapper.Map<PaymentDto>(p)).ToList();
            return Ok(dtos);
        }

        [HttpPost("{id:guid}/confirm-payment")]
        [Authorize(Roles = "HospitalAdmin,InsuranceAgency")]
        public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] ConfirmPaymentDto request, CancellationToken cancellationToken)
        {
            var payment = await _mediator.Send(new GetByIdQuery<Domain.Entities.Payment>(request.PaymentId), cancellationToken);
            if (payment is null) return NotFound();

            if (payment.PreContractId != id)
                return BadRequest("Payment does not belong to this pre-contract");

            payment.Confirmed = true;
            payment.PaidAtUtc = DateTime.UtcNow;
            
            await _mediator.Send(new UpdateCommand<Domain.Entities.Payment>(payment), cancellationToken);

            return Ok(_mapper.Map<PaymentDto>(payment));
        }

        private string GeneratePaymentPlan(decimal totalAmount, string planType)
        {
            return planType.ToLower() switch
            {
                "single" => $"1x{totalAmount:F2}",
                "monthly" => $"3x{totalAmount / 3:F2}",
                "quarterly" => $"4x{totalAmount / 4:F2}",
                _ => $"2x{totalAmount / 2:F2}"
            };
        }

        private async Task CreatePaymentInstallments(Domain.Entities.PreContract preContract, CancellationToken cancellationToken)
        {
            var planParts = preContract.PaymentPlan.Split('x');
            if (planParts.Length != 2) return;

            var installmentCount = int.Parse(planParts[0]);
            var installmentAmount = decimal.Parse(planParts[1]);

            for (int i = 0; i < installmentCount; i++)
            {
                var payment = new Domain.Entities.Payment
                {
                    Id = Guid.NewGuid(),
                    PreContractId = preContract.Id,
                    Amount = installmentAmount,
                    DueDateUtc = DateTime.UtcNow.AddDays(30 * (i + 1)), // 30 days between installments
                    Confirmed = false,
                    LateCount = 0
                };

                await _mediator.Send(new CreateCommand<Domain.Entities.Payment>(payment), cancellationToken);
            }
        }

        private async Task ProcessRefunds(Domain.Entities.PreContract preContract, decimal refundPercentage, CancellationToken cancellationToken)
        {
            var payments = await _paymentDb.GetAllAsync(cancellationToken);
            var contractPayments = payments.Where(p => p.PreContractId == preContract.Id && p.Confirmed);

            foreach (var payment in contractPayments)
            {
                var refundAmount = payment.Amount * (refundPercentage / 100);
            }
        }
    }

    public class TerminatePreContractDto
    {
        public decimal RefundPercentage { get; set; } = 80; // Default 80% refund
        public string Reason { get; set; } = string.Empty;
    }

    public class ConfirmPaymentDto
    {
        public Guid PaymentId { get; set; }
    }
}
