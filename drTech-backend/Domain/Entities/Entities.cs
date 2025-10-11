namespace drTech_backend.Domain.Entities
{
    public class Hospital
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public ICollection<Department> Departments { get; set; } = new List<Department>();
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
    }

    public class Equipment
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Operational";
        public Guid DepartmentId { get; set; }
        public bool IsWithdrawn { get; set; }
    }

    public class Patient
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
    }

    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty; // exam, surgery, lab
        public bool IsConfirmed { get; set; }
    }
}


