using System;
using System.Collections.Generic;

namespace drTech_backend.Application.Common.DTOs
{
    // Hospitals / Departments / Doctors
    public class HospitalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int DepartmentsCount { get; set; }
    }

    public class HospitalCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public Guid HospitalId { get; set; }
    }

    public class DepartmentCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public Guid HospitalId { get; set; }
    }

    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    public class DoctorCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    // Equipment
    public class EquipmentDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public bool IsWithdrawn { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
    }

    public class EquipmentCreateDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Operational";
        public Guid DepartmentId { get; set; }
        public bool IsWithdrawn { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
    }

    // Patients
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
    }

    public class PatientCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
        public Guid? InsuranceAgencyId { get; set; }
        public Guid UserId { get; set; }
    }

    // Appointments
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public int RescheduleCount { get; set; }
        public string? Notes { get; set; }
        public List<Guid> RequiredEquipmentIds { get; set; } = new List<Guid>();
    }

    public class AppointmentCreateDto
    {
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? MedicalServiceId { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public bool IsConfirmed { get; set; }
        public int RescheduleCount { get; set; } = 0;
        public string? Notes { get; set; }
        public List<Guid> RequiredEquipmentIds { get; set; } = new List<Guid>();
    }
}


