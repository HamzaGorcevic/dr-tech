namespace drTech_backend.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "InsuredUser"; // HospitalAdmin/Doctor/InsuranceAgency/InsuredUser
        public string FullName { get; set; } = string.Empty;
        
        // Foreign Keys
        public Guid? HospitalId { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }
        
        // Navigation Properties
        public Hospital? Hospital { get; set; }
        public InsuranceAgency? InsuranceAgency { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public bool Revoked { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }

    public class AuditLog
    {
        public Guid Id { get; set; }
        public string Actor { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // Create/Update/Delete
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string? Description { get; set; }
    }
}


