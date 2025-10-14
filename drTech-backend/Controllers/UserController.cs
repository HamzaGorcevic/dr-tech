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
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabaseService<Domain.Entities.User> _userDb;
        private readonly IDatabaseService<Domain.Entities.Patient> _patientDb;
        private readonly IDatabaseService<Domain.Entities.Hospital> _hospitalDb;
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _agencyDb;
        private readonly IDatabaseService<Domain.Entities.MedicalService> _serviceDb;
        private readonly IDatabaseService<Domain.Entities.PriceListItem> _priceListDb;
        private readonly IDatabaseService<Domain.Entities.Appointment> _appointmentDb;
        private readonly IDatabaseService<Domain.Entities.Discount> _discountDb;

        public UserController(
            IMediator mediator,
            IDatabaseService<Domain.Entities.User> userDb,
            IDatabaseService<Domain.Entities.Patient> patientDb,
            IDatabaseService<Domain.Entities.Hospital> hospitalDb,
            IDatabaseService<Domain.Entities.InsuranceAgency> agencyDb,
            IDatabaseService<Domain.Entities.MedicalService> serviceDb,
            IDatabaseService<Domain.Entities.PriceListItem> priceListDb,
            IDatabaseService<Domain.Entities.Appointment> appointmentDb,
            IDatabaseService<Domain.Entities.Discount> discountDb)
        {
            _mediator = mediator;
            _userDb = userDb;
            _patientDb = patientDb;
            _hospitalDb = hospitalDb;
            _agencyDb = agencyDb;
            _serviceDb = serviceDb;
            _priceListDb = priceListDb;
            _appointmentDb = appointmentDb;
            _discountDb = discountDb;
        }

        [HttpGet("profile")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null) return NotFound();

            var patient = user.PatientId.HasValue 
                ? await _mediator.Send(new GetByIdQuery<Domain.Entities.Patient>(user.PatientId.Value), cancellationToken)
                : null;

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Patient = patient
            };

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null) return NotFound();

            user.FullName = request.FullName;

            if (user.PatientId.HasValue)
            {
                var patient = await _mediator.Send(new GetByIdQuery<Domain.Entities.Patient>(user.PatientId.Value), cancellationToken);
                if (patient is not null)
                {
                    patient.FullName = request.FullName;
                    patient.Allergies = request.Allergies;
                    patient.MedicalHistory = request.MedicalHistory;
                    patient.CurrentTherapies = request.CurrentTherapies;
                    await _mediator.Send(new UpdateCommand<Domain.Entities.Patient>(patient), cancellationToken);
                }
            }

            await _mediator.Send(new UpdateCommand<Domain.Entities.User>(user), cancellationToken);
            return Ok(user);
        }

        [HttpGet("hospitals")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetHospitals([FromQuery] string? city, CancellationToken cancellationToken)
        {
            var hospitals = await _hospitalDb.GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(city))
                hospitals = hospitals.Where(h => h.City.Contains(city, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(hospitals);
        }

        [HttpGet("agencies")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetAgencies([FromQuery] string? city, CancellationToken cancellationToken)
        {
            var agencies = await _agencyDb.GetAllAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(city))
                agencies = agencies.Where(a => a.City.Contains(city, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(agencies);
        }

        [HttpGet("services")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetServices([FromQuery] ServiceFilterDto filter, CancellationToken cancellationToken)
        {
            var services = await _serviceDb.GetAllAsync(cancellationToken);
            var priceList = await _priceListDb.GetAllAsync(cancellationToken);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.ServiceType))
                services = services.Where(s => s.Type == filter.ServiceType).ToList();

            if (filter.HospitalId.HasValue)
            {
                var hospitalPrices = priceList.Where(p => p.HospitalId == filter.HospitalId.Value);
                var serviceIds = hospitalPrices.Select(p => p.MedicalServiceId);
                services = services.Where(s => serviceIds.Contains(s.Id)).ToList();
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                var hospitals = await _hospitalDb.GetAllAsync(cancellationToken);
                var cityHospitals = hospitals.Where(h => h.City.Contains(filter.City, StringComparison.OrdinalIgnoreCase));
                var cityHospitalIds = cityHospitals.Select(h => h.Id);
                var cityPrices = priceList.Where(p => cityHospitalIds.Contains(p.HospitalId));
                var cityServiceIds = cityPrices.Select(p => p.MedicalServiceId);
                services = services.Where(s => cityServiceIds.Contains(s.Id)).ToList();
            }

            // Add pricing information
            var result = services.Select(service =>
            {
                var prices = priceList.Where(p => p.MedicalServiceId == service.Id && p.IsActive);
                return new ServiceWithPricingDto
                {
                    Id = service.Id,
                    Code = service.Code,
                    Name = service.Name,
                    Type = service.Type,
                    DepartmentId = service.DepartmentId,
                    Prices = prices.Select(p => new ServicePriceDto
                    {
                        HospitalId = p.HospitalId,
                        Price = p.Price,
                        ValidFrom = p.ValidFrom,
                        ValidUntil = p.ValidUntil
                    }).ToList()
                };
            });

            return Ok(result.ToList());
        }

        [HttpPost("request-appointment")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> RequestAppointment([FromBody] RequestAppointmentDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null || !user.PatientId.HasValue) return BadRequest("User not associated with patient");

            // Check for existing active appointments
            var existingAppointments = await _appointmentDb.GetAllAsync(cancellationToken);
            var activeAppointments = existingAppointments.Where(a => 
                a.PatientId == user.PatientId.Value && 
                a.Status != "Cancelled" &&
                a.StartsAtUtc < request.EndsAtUtc && 
                request.StartsAtUtc < a.EndsAtUtc);

            if (activeAppointments.Any())
                return Conflict("Patient already has an active appointment in this period");

            var appointment = new Domain.Entities.Appointment
            {
                Id = Guid.NewGuid(),
                HospitalId = request.HospitalId,
                DepartmentId = request.DepartmentId,
                DoctorId = request.DoctorId,
                PatientId = user.PatientId.Value,
                MedicalServiceId = request.MedicalServiceId,
                StartsAtUtc = request.StartsAtUtc,
                EndsAtUtc = request.EndsAtUtc,
                Type = request.Type,
                Status = "Requested",
                IsConfirmed = false,
                Notes = request.Notes
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.Appointment>(appointment), cancellationToken);
            return CreatedAtAction(nameof(GetAppointments), new { }, appointment);
        }

        [HttpGet("appointments")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null || !user.PatientId.HasValue) return BadRequest("User not associated with patient");

            var appointments = await _appointmentDb.GetAllAsync(cancellationToken);
            var userAppointments = appointments
                .Where(a => a.PatientId == user.PatientId.Value)
                .OrderByDescending(a => a.StartsAtUtc);

            return Ok(userAppointments);
        }

        [HttpGet("discounts")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> GetDiscounts(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null || !user.PatientId.HasValue) return BadRequest("User not associated with patient");

            var discounts = await _discountDb.GetAllAsync(cancellationToken);
            var userDiscounts = discounts
                .Where(d => d.PatientId == user.PatientId.Value && d.IsActive && d.ValidUntil > DateTime.UtcNow)
                .OrderByDescending(d => d.ValidFrom);

            return Ok(userDiscounts);
        }

        [HttpPost("request-discount")]
        [Authorize(Roles = "InsuredUser")]
        public async Task<IActionResult> RequestDiscount([FromBody] RequestDiscountDto request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _mediator.Send(new GetByIdQuery<Domain.Entities.User>(userGuid), cancellationToken);
            if (user is null || !user.PatientId.HasValue) return BadRequest("User not associated with patient");

            if (!user.InsuranceAgencyId.HasValue)
                return BadRequest("User not associated with insurance agency");

            var discountRequest = new Domain.Entities.DiscountRequest
            {
                Id = Guid.NewGuid(),
                InsuranceAgencyId = user.InsuranceAgencyId.Value,
                HospitalId = request.HospitalId,
                PatientId = user.PatientId.Value,
                RequestedDiscountPercent = request.RequestedDiscountPercent,
                Reason = request.Reason,
                Explanation = request.Explanation,
                Status = "Pending",
                RequestedAtUtc = DateTime.UtcNow
            };

            await _mediator.Send(new CreateCommand<Domain.Entities.DiscountRequest>(discountRequest), cancellationToken);
            return CreatedAtAction(nameof(GetDiscounts), new { }, discountRequest);
        }
    }

    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Domain.Entities.Patient? Patient { get; set; }
    }

    public class UpdateUserProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
    }

    public class ServiceFilterDto
    {
        public string? ServiceType { get; set; }
        public Guid? HospitalId { get; set; }
        public string? City { get; set; }
        public string? Specialist { get; set; }
        public DateTime? Date { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class ServiceWithPricingDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public List<ServicePriceDto> Prices { get; set; } = new List<ServicePriceDto>();
    }

    public class ServicePriceDto
    {
        public Guid HospitalId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    public class RequestAppointmentDto
    {
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class RequestDiscountDto
    {
        public Guid HospitalId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }
}
