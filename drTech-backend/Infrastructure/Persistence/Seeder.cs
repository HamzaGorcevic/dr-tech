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

            // Users with different roles
            Guid hospitalAdminUserId = Guid.Empty;
            Guid doctorUserId = Guid.Empty;
            Guid agencyRepUserId = Guid.Empty;
            Guid insuredUserId = Guid.Empty;

            if (!await db.Users.AnyAsync())
            {
                // Hospital Admin
                var hospitalAdmin = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "hospital.admin@drtech.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin#123"),
                    Role = "HospitalAdmin",
                    FullName = "Hospital Administrator",
                    CreatedAtUtc = DateTime.UtcNow
                };
                await db.Users.AddAsync(hospitalAdmin);
                hospitalAdminUserId = hospitalAdmin.Id;

                // Doctor
                var doctor = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "doctor@drtech.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor#123"),
                    Role = "Doctor",
                    FullName = "Dr. John Doe",
                    CreatedAtUtc = DateTime.UtcNow
                };
                await db.Users.AddAsync(doctor);
                doctorUserId = doctor.Id;

                // Insurance Agency Representative
                var agencyRep = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "agency@drtech.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agency#123"),
                    Role = "InsuranceAgency",
                    FullName = "Insurance Agency Rep",
                    CreatedAtUtc = DateTime.UtcNow
                };
                await db.Users.AddAsync(agencyRep);
                agencyRepUserId = agencyRep.Id;

                // Insured User
                var insuredUser = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "patient@drtech.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient#123"),
                    Role = "InsuredUser",
                    FullName = "Jane Smith",
                    CreatedAtUtc = DateTime.UtcNow
                };
                await db.Users.AddAsync(insuredUser);
                insuredUserId = insuredUser.Id;
            }
            else
            {
                // Get existing user IDs
                var existingUsers = await db.Users.ToListAsync();
                hospitalAdminUserId = existingUsers.FirstOrDefault(u => u.Role == "HospitalAdmin")?.Id ?? Guid.Empty;
                doctorUserId = existingUsers.FirstOrDefault(u => u.Role == "Doctor")?.Id ?? Guid.Empty;
                agencyRepUserId = existingUsers.FirstOrDefault(u => u.Role == "InsuranceAgency")?.Id ?? Guid.Empty;
                insuredUserId = existingUsers.FirstOrDefault(u => u.Role == "InsuredUser")?.Id ?? Guid.Empty;
            }

            // Hospitals & Departments
            Domain.Entities.Hospital? hospitalEntity = await db.Hospitals.FirstOrDefaultAsync();
            if (hospitalEntity == null)
            {
                hospitalEntity = new Domain.Entities.Hospital
                {
                    Id = Guid.NewGuid(),
                    Name = "City Hospital",
                    City = "Belgrade",
                    UserId = hospitalAdminUserId
                };
                await db.Hospitals.AddAsync(hospitalEntity);
                await db.SaveChangesAsync(); 
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
                await db.SaveChangesAsync(); // Save Department first so Doctor can reference it
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
                    DepartmentId = departmentEntity.Id,
                    UserId = doctorUserId
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
                    Allergies = "None",
                    UserId = insuredUserId
                };
                await db.Patients.AddAsync(patientEntity);
                await db.SaveChangesAsync(); 
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
                    City = "Belgrade",
                    UserId = agencyRepUserId
                };
                await db.InsuranceAgencies.AddAsync(agencyEntity);
                await db.SaveChangesAsync(); 
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

            // Equipment Status Logs
            if (!await db.EquipmentStatusLogs.AnyAsync())
            {
                await db.EquipmentStatusLogs.AddAsync(new Domain.Entities.EquipmentStatusLog
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = equipmentEntity.Id,
                    Status = "Operational",
                    Note = "Equipment registered and operational",
                    LoggedAtUtc = DateTime.UtcNow
                });
            }

            // Equipment Service Orders
            if (!await db.EquipmentServiceOrders.AnyAsync())
            {
                await db.EquipmentServiceOrders.AddAsync(new Domain.Entities.EquipmentServiceOrder
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = equipmentEntity.Id,
                    Type = "Service",
                    ScheduledAtUtc = DateTime.UtcNow.AddMonths(6),
                    Status = "Scheduled"
                });
            }

            // Discounts
            if (!await db.Discounts.AnyAsync())
            {
                await db.Discounts.AddAsync(new Domain.Entities.Discount
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientEntity.Id,
                    HospitalId = hospitalEntity.Id,
                    DiscountPercent = 10,
                    MaxDiscountAmount = 100,
                    Reason = "TotalValue",
                    ValidFrom = DateTime.UtcNow,
                    ValidUntil = DateTime.UtcNow.AddMonths(6),
                    IsActive = true,
                    Status = "Approved"
                });
            }

            // Discount Requests
            if (!await db.DiscountRequests.AnyAsync())
            {
                await db.DiscountRequests.AddAsync(new Domain.Entities.DiscountRequest
                {
                    Id = Guid.NewGuid(),
                    InsuranceAgencyId = agencyEntity.Id,
                    HospitalId = hospitalEntity.Id,
                    PatientId = patientEntity.Id,
                    RequestedDiscountPercent = 15,
                    Reason = "Children",
                    Explanation = "Patient is under 18 years old",
                    Status = "Pending",
                    RequestedAtUtc = DateTime.UtcNow
                });
            }

            // Error Logs
            if (!await db.ErrorLogs.AnyAsync())
            {
                await db.ErrorLogs.AddAsync(new Domain.Entities.ErrorLog
                {
                    Id = Guid.NewGuid(),
                    ErrorType = "4xx",
                    StatusCode = 404,
                    Message = "Resource not found",
                    RequestPath = "/api/test/notfound",
                    RequestMethod = "GET",
                    UserId = "system",
                    OccurredAtUtc = DateTime.UtcNow.AddHours(-1)
                });
            }

            // Request Logs
            if (!await db.RequestLogs.AnyAsync())
            {
                await db.RequestLogs.AddAsync(new Domain.Entities.RequestLog
                {
                    Id = Guid.NewGuid(),
                    UserId = "system",
                    IpAddress = "127.0.0.1",
                    Endpoint = "/api/hospitals",
                    HttpMethod = "GET",
                    StatusCode = 200,
                    ResponseTimeMs = 150,
                    TimestampUtc = DateTime.UtcNow.AddMinutes(-30)
                });
            }

            // Throttle Logs
            if (!await db.ThrottleLogs.AnyAsync())
            {
                await db.ThrottleLogs.AddAsync(new Domain.Entities.ThrottleLog
                {
                    Id = Guid.NewGuid(),
                    UserId = "test-user",
                    IpAddress = "192.168.1.100",
                    RequestCount = 105,
                    WindowStartUtc = DateTime.UtcNow.AddMinutes(-10),
                    WindowEndUtc = DateTime.UtcNow,
                    IsBlocked = true,
                    BlockedUntilUtc = DateTime.UtcNow.AddMinutes(5)
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
                    Description = "Initial seed completed with comprehensive data"
                });
            }

            // Update User entities with foreign key relationships
            if (hospitalAdminUserId != Guid.Empty && hospitalEntity != null)
            {
                var hospitalAdminUser = await db.Users.FindAsync(hospitalAdminUserId);
                if (hospitalAdminUser != null)
                {
                    hospitalAdminUser.HospitalId = hospitalEntity.Id;
                }
            }

            if (doctorUserId != Guid.Empty && doctorId != Guid.Empty)
            {
                var doctorUser = await db.Users.FindAsync(doctorUserId);
                if (doctorUser != null)
                {
                    doctorUser.DoctorId = doctorId;
                }
            }

            if (insuredUserId != Guid.Empty && patientEntity != null)
            {
                var insuredUser = await db.Users.FindAsync(insuredUserId);
                if (insuredUser != null)
                {
                    insuredUser.PatientId = patientEntity.Id;
                }
            }

            if (agencyRepUserId != Guid.Empty && agencyEntity != null)
            {
                var agencyUser = await db.Users.FindAsync(agencyRepUserId);
                if (agencyUser != null)
                {
                    agencyUser.InsuranceAgencyId = agencyEntity.Id;
                }
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Database migrated and seeded with comprehensive data");
        }
    }
}


