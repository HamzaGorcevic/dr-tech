using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace drTech_backend.Infrastructure.Mongo
{
    // MongoDB models mirroring PostgreSQL entities
    public class MongoHospital
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid? UserId { get; set; } // For HospitalAdmin role
        public List<MongoDepartment> Departments { get; set; } = new();
    }

    public class MongoDepartment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        public List<MongoDoctor> Doctors { get; set; } = new();
        public List<MongoEquipment> Equipment { get; set; } = new();
    }

    public class MongoDoctor
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid DepartmentId { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
    }

    public class MongoPatient
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? InsuranceAgencyId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public List<MongoReservation> Reservations { get; set; } = new();
    }

    public class MongoEquipment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Operational";
        [BsonRepresentation(BsonType.String)]
        public Guid DepartmentId { get; set; }
        public bool IsWithdrawn { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
        public List<MongoEquipmentStatusLog> StatusLogs { get; set; } = new();
    }

    public class MongoReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid DepartmentId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class MongoInsuranceAgency
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid? UserId { get; set; } // For InsuranceAgency role
        public List<MongoAgencyContract> Contracts { get; set; } = new();
    }

    public class MongoAgencyContract
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid InsuranceAgencyId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        public decimal CoveragePercent { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public string Status { get; set; } = "Proposed";
        public string? RejectionReason { get; set; }
    }

    public class MongoMedicalService
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid DepartmentId { get; set; }
    }

    public class MongoPriceListItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid MedicalServiceId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
    }

    public class MongoPayment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PreContractId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDateUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
        public string? ProofUrl { get; set; }
        public bool Confirmed { get; set; }
        public int LateCount { get; set; }
    }

    public class MongoEquipmentStatusLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid EquipmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime LoggedAtUtc { get; set; }
    }

    public class MongoEquipmentServiceOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid EquipmentId { get; set; }
        public string Type { get; set; } = "Service";
        public DateTime ScheduledAtUtc { get; set; }
        public string Status { get; set; } = "Scheduled";
    }

    public class MongoPreContract
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid InsuranceAgencyId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PaymentPlan { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class MongoUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "InsuredUser";
        public string FullName { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid? HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? InsuranceAgencyId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? DoctorId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? PatientId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
        public List<MongoRefreshToken> RefreshTokens { get; set; } = new();
    }

    public class MongoRefreshToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public bool Revoked { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
    }

    // New entities for enhanced functionality
    public class MongoAppointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid DepartmentId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid DoctorId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public string Status { get; set; } = "Scheduled";
        public int RescheduleCount { get; set; }
        public string? Notes { get; set; }
        public List<Guid> RequiredEquipmentIds { get; set; } = new();
    }

    public class MongoDiscount
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid InsuranceAgencyId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MongoDiscountRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid InsuranceAgencyId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid HospitalId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }

    public class MongoErrorLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string ErrorType { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string RequestPath { get; set; } = string.Empty;
        public string RequestMethod { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime OccurredAtUtc { get; set; }
    }

    public class MongoRequestLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    public class MongoThrottleLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public DateTime WindowStartUtc { get; set; }
        public DateTime WindowEndUtc { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntilUtc { get; set; }
    }

    public class MongoAuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Actor { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string? Description { get; set; }
    }
}
