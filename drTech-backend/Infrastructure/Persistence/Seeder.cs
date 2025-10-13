using Microsoft.EntityFrameworkCore;

namespace drTech_backend.Infrastructure
{
    public static class Seeder
    {
        public static async Task MigrateAndSeedAsync(this IApplicationBuilder app, ILogger logger)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Skip migrations if schema already exists (e.g., created earlier)
            var schemaExists = false;
            try
            {
                // If this succeeds, tables already exist
                schemaExists = await db.Users.AnyAsync();
            }
            catch
            {
                schemaExists = false;
            }

            if (!schemaExists)
            {
                await db.Database.MigrateAsync();
            }

            // Users
            if (!await db.Users.AnyAsync())
            {
                var admin = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@drtech.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin#123"),
                    Role = "Admin"
                };
                await db.Users.AddAsync(admin);
            }

            // Hospitals & Departments
            Domain.Entities.Hospital? hospitalEntity = await db.Hospitals.FirstOrDefaultAsync();
            if (hospitalEntity == null)
            {
                hospitalEntity = new Domain.Entities.Hospital
                {
                    Id = Guid.NewGuid(),
                    Name = "City Hospital",
                    City = "Belgrade"
                };
                await db.Hospitals.AddAsync(hospitalEntity);
            }

            Domain.Entities.Department? departmentEntity = await db.Departments.FirstOrDefaultAsync();
            if (departmentEntity == null)
            {
                departmentEntity = new Domain.Entities.Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Surgery",
                    DoctorsCount = 5,
                    HospitalId = hospitalEntity.Id
                };
                await db.Departments.AddAsync(departmentEntity);
            }

            // Doctors
            Guid doctorId;
            var existingDoctor = await db.Doctors.FirstOrDefaultAsync();
            if (existingDoctor == null)
            {
                var newDoctor = new Domain.Entities.Doctor
                {
                    Id = Guid.NewGuid(),
                    FullName = "Dr. John Doe",
                    Specialty = "General Surgery",
                    DepartmentId = departmentEntity.Id
                };
                await db.Doctors.AddAsync(newDoctor);
                // ensure persisted so subsequent queries see it
                await db.SaveChangesAsync();
                doctorId = newDoctor.Id;
            }
            else
            {
                doctorId = existingDoctor.Id;
            }

            // Patients
            Domain.Entities.Patient? patientEntity = await db.Patients.FirstOrDefaultAsync();
            if (patientEntity == null)
            {
                patientEntity = new Domain.Entities.Patient
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Smith",
                    InsuranceNumber = "INS-0001",
                    Allergies = "None"
                };
                await db.Patients.AddAsync(patientEntity);
            }

            // Equipment
            Domain.Entities.Equipment? equipmentEntity = await db.Equipment.FirstOrDefaultAsync();
            if (equipmentEntity == null)
            {
                equipmentEntity = new Domain.Entities.Equipment
                {
                    Id = Guid.NewGuid(),
                    SerialNumber = "EQ-1001",
                    Type = "X-Ray",
                    Status = "Operational",
                    DepartmentId = departmentEntity.Id,
                    IsWithdrawn = false
                };
                await db.Equipment.AddAsync(equipmentEntity);
            }

            // Appointments
            if (!await db.Appointments.AnyAsync())
            {
                await db.Appointments.AddAsync(new Domain.Entities.Appointment
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalEntity.Id,
                    DepartmentId = departmentEntity.Id,
                    DoctorId = doctorId,
                    PatientId = patientEntity.Id,
                    StartsAtUtc = DateTime.UtcNow.AddDays(1),
                    EndsAtUtc = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Type = "exam",
                    IsConfirmed = true
                });
            }

            // Insurance Agencies
            Domain.Entities.InsuranceAgency? agencyEntity = await db.InsuranceAgencies.FirstOrDefaultAsync();
            if (agencyEntity == null)
            {
                agencyEntity = new Domain.Entities.InsuranceAgency
                {
                    Id = Guid.NewGuid(),
                    Name = "Sample Insurance Agency",
                    City = "Belgrade"
                };
                await db.InsuranceAgencies.AddAsync(agencyEntity);
            }

            // Agency Contracts
            if (!await db.AgencyContracts.AnyAsync())
            {
                await db.AgencyContracts.AddAsync(new Domain.Entities.AgencyContract
                {
                    Id = Guid.NewGuid(),
                    InsuranceAgencyId = agencyEntity.Id,
                    HospitalId = hospitalEntity.Id,
                    CoveragePercent = 0.5m,
                    StartsOn = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc),
                    EndsOn = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddYears(1), DateTimeKind.Utc),
                    Status = "Accepted"
                });
            }

            // Medical Services
            Domain.Entities.MedicalService? serviceEntity = await db.MedicalServices.FirstOrDefaultAsync();
            if (serviceEntity == null)
            {
                serviceEntity = new Domain.Entities.MedicalService
                {
                    Id = Guid.NewGuid(),
                    Code = "SRV-001",
                    Name = "General Checkup",
                    Type = "exam",
                    DepartmentId = departmentEntity.Id
                };
                await db.MedicalServices.AddAsync(serviceEntity);
            }

            // Price List
            if (!await db.PriceList.AnyAsync())
            {
                await db.PriceList.AddAsync(new Domain.Entities.PriceListItem
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalEntity.Id,
                    MedicalServiceId = serviceEntity.Id,
                    Price = 100,
                    ValidFrom = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(DateTime.UtcNow.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    IsActive = true
                });
            }

            // Reservations
            if (!await db.Reservations.AnyAsync())
            {
                await db.Reservations.AddAsync(new Domain.Entities.Reservation
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalEntity.Id,
                    DepartmentId = departmentEntity.Id,
                    PatientId = patientEntity.Id,
                    MedicalServiceId = serviceEntity.Id,
                    StartsAtUtc = DateTime.UtcNow.AddDays(2),
                    EndsAtUtc = DateTime.UtcNow.AddDays(2).AddHours(1),
                    Status = "Pending"
                });
            }

            // PreContracts
            Domain.Entities.PreContract? preContractEntity = await db.PreContracts.FirstOrDefaultAsync();
            if (preContractEntity == null)
            {
                preContractEntity = new Domain.Entities.PreContract
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalEntity.Id,
                    InsuranceAgencyId = agencyEntity.Id,
                    PatientId = patientEntity.Id,
                    AgreedPrice = 500,
                    PaymentPlan = "2x250",
                    CreatedAtUtc = DateTime.UtcNow,
                    Status = "Active"
                };
                await db.PreContracts.AddAsync(preContractEntity);
            }

            // Payments
            if (!await db.Payments.AnyAsync())
            {
                await db.Payments.AddAsync(new Domain.Entities.Payment
                {
                    Id = Guid.NewGuid(),
                    PreContractId = preContractEntity.Id,
                    Amount = 250,
                    DueDateUtc = DateTime.UtcNow.AddDays(7),
                    Confirmed = false,
                    LateCount = 0
                });
            }

            // Audit Logs
            if (!await db.AuditLogs.AnyAsync())
            {
                await db.AuditLogs.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Actor = "system",
                    Action = "Seed",
                    Path = "/seed",
                    Method = "SYSTEM",
                    StatusCode = 200,
                    OccurredAtUtc = DateTime.UtcNow,
                    Description = "Initial seed completed"
                });
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Database migrated and seeded");
        }
    }
}


