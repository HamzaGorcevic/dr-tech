namespace drTech_backend.Domain.Entities
{
    public class Hospital
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        
        // Navigation Properties
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public Guid? UserId { get; set; } // For HospitalAdmin role
        public User? User { get; set; }
    }

    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public Guid HospitalId { get; set; }
        public Hospital? Hospital { get; set; }
    }

    public class Doctor
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string WorkingHours { get; set; } = string.Empty; // e.g., "9:00-17:00"
        public bool IsAvailable { get; set; } = true;
        
        // Navigation Properties
        public Department? Department { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }

    public class Equipment
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Operational"; // Operational/Faulty/Withdrawn
        public Guid DepartmentId { get; set; }
        public bool IsWithdrawn { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
        public Department? Department { get; set; }
    }

    public class Patient
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        
        // Navigation Properties
        public InsuranceAgency? InsuranceAgency { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }

    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty; // exam, surgery, lab
        public string Status { get; set; } = "Scheduled"; // Scheduled/Confirmed/Cancelled/Completed
        public bool IsConfirmed { get; set; }
        public int RescheduleCount { get; set; } = 0;
        public string? Notes { get; set; }
        public List<Guid> RequiredEquipmentIds { get; set; } = new List<Guid>();
    }
}


