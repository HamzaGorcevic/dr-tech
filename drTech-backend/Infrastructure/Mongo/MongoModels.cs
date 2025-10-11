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
    }

    public class MongoPatient
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
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

    public class MongoUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
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
}
