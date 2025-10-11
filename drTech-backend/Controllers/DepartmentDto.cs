namespace drTech_backend.Controllers
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public Guid HospitalId { get; set; }
    }

    public class DepartmentResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public Guid HospitalId { get; set; }
        public string HospitalName { get; set; } = string.Empty;
    }

    public class CreateHospitalDto
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class HospitalResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int DepartmentsCount { get; set; }
    }
}
