namespace drTech_backend.Domain.Entities
{
    public class InsuranceAgency
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        
        // Navigation Properties
        public ICollection<AgencyContract> Contracts { get; set; } = new List<AgencyContract>();
        public Guid? UserId { get; set; } // For InsuranceAgency role
        public User? User { get; set; }
    }

    public class AgencyContract
    {
        public Guid Id { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public decimal CoveragePercent { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public string Status { get; set; } = "Proposed"; // Proposed/Accepted/Rejected
        public string? RejectionReason { get; set; }
    }

    public class MedicalService
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // exam/surgery/lab
        public Guid DepartmentId { get; set; }
    }

    public class PriceListItem
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; } // end of year
        public bool IsActive { get; set; }
    }

    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Status { get; set; } = "Pending"; // Pending/Confirmed/Cancelled
    }

    public class PreContract
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid PatientId { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PaymentPlan { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public string Status { get; set; } = "Active"; // Active/Terminated/Completed
    }

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid PreContractId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDateUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
        public string? ProofUrl { get; set; } // file path or url
        public bool Confirmed { get; set; }
        public int LateCount { get; set; }
    }

    public class EquipmentStatusLog
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public string Status { get; set; } = string.Empty; // Operational/OutOfService/Withdrawn
        public string? Note { get; set; }
        public DateTime LoggedAtUtc { get; set; }
    }

    public class EquipmentServiceOrder
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public string Type { get; set; } = "Service"; // Service/Replacement
        public DateTime ScheduledAtUtc { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled/Completed/Cancelled
    }

    public class Discount
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? HospitalId { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty; // Reschedule/TotalValue/Special
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected
    }

    public class DiscountRequest
    {
        public Guid Id { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public Guid PatientId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty; // Children/Disabled/Special
        public string Explanation { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected
        public string? RejectionReason { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }

    public class ErrorLog
    {
        public Guid Id { get; set; }
        public string ErrorType { get; set; } = string.Empty; // 4xx/5xx
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public string? UserId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
    }

    public class RequestLog
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    public class ThrottleLog
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public DateTime WindowStartUtc { get; set; }
        public DateTime WindowEndUtc { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntilUtc { get; set; }
    }
}


