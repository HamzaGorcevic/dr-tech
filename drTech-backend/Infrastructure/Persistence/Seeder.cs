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

            if (!await db.Hospitals.AnyAsync())
            {
                var hospital = new Domain.Entities.Hospital
                {
                    Id = Guid.NewGuid(),
                    Name = "City Hospital",
                    City = "Belgrade"
                };
                var department = new Domain.Entities.Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Surgery",
                    DoctorsCount = 5,
                    HospitalId = hospital.Id
                };
                await db.Hospitals.AddAsync(hospital);
                await db.Departments.AddAsync(department);
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Database migrated and seeded");
        }
    }
}


