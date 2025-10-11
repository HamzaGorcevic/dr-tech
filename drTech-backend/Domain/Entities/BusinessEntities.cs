namespace drTech_backend.Domain.Entities
{
    public class InsuranceAgency
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public ICollection<AgencyContract> Contracts { get; set; } = new List<AgencyContract>();
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
}


