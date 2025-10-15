using System;

namespace drTech_backend.Application.Common.DTOs
{
    public class InsuranceAgencyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int ContractsCount { get; set; }
    }

    public class InsuranceAgencyCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class AgencyContractDto
    {
        public Guid Id { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public decimal CoveragePercent { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }

    public class AgencyContractCreateDto
    {
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public decimal CoveragePercent { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public string Status { get; set; } = "Proposed";
        public string? RejectionReason { get; set; }
    }

    public class MedicalServiceDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
    }

    public class MedicalServiceCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
    }

    public class PriceListItemDto
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
    }

    public class PriceListItemCreateDto
    {
        public Guid HospitalId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ReservationCreateDto
    {
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class PreContractDto
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid PatientId { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PaymentPlan { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PreContractCreateDto
    {
        public Guid HospitalId { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid PatientId { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PaymentPlan { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid PreContractId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDateUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
        public string? ProofUrl { get; set; }
        public bool Confirmed { get; set; }
        public int LateCount { get; set; }
    }

    public class PaymentCreateDto
    {
        public Guid PreContractId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDateUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
        public string? ProofUrl { get; set; }
        public bool Confirmed { get; set; }
        public int LateCount { get; set; }
    }

    public class EquipmentStatusLogDto
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime LoggedAtUtc { get; set; }
    }

    public class EquipmentStatusLogCreateDto
    {
        public Guid EquipmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime LoggedAtUtc { get; set; }
    }

    public class EquipmentServiceOrderDto
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime ScheduledAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EquipmentServiceOrderCreateDto
    {
        public Guid EquipmentId { get; set; }
        public string Type { get; set; } = "Service";
        public DateTime ScheduledAtUtc { get; set; }
        public string Status { get; set; } = "Scheduled";
    }

    public class DiscountDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? HospitalId { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DiscountCreateDto
    {
        public Guid PatientId { get; set; }
        public Guid? HospitalId { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Pending";
    }

    public class DiscountRequestDto
    {
        public Guid Id { get; set; }
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public Guid PatientId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }

    public class DiscountRequestCreateDto
    {
        public Guid InsuranceAgencyId { get; set; }
        public Guid HospitalId { get; set; }
        public Guid PatientId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? RejectionReason { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }
}


