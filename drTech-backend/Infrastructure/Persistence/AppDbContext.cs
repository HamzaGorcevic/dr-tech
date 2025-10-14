using Microsoft.EntityFrameworkCore;

namespace drTech_backend.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Hospital> Hospitals => Set<Domain.Entities.Hospital>();
        public DbSet<Domain.Entities.Department> Departments => Set<Domain.Entities.Department>();
        public DbSet<Domain.Entities.Doctor> Doctors => Set<Domain.Entities.Doctor>();
        public DbSet<Domain.Entities.Patient> Patients => Set<Domain.Entities.Patient>();
        public DbSet<Domain.Entities.Equipment> Equipment => Set<Domain.Entities.Equipment>();
        public DbSet<Domain.Entities.Appointment> Appointments => Set<Domain.Entities.Appointment>();
        public DbSet<Domain.Entities.User> Users => Set<Domain.Entities.User>();
        public DbSet<Domain.Entities.RefreshToken> RefreshTokens => Set<Domain.Entities.RefreshToken>();
        public DbSet<Domain.Entities.AuditLog> AuditLogs => Set<Domain.Entities.AuditLog>();
        public DbSet<Domain.Entities.InsuranceAgency> InsuranceAgencies => Set<Domain.Entities.InsuranceAgency>();
        public DbSet<Domain.Entities.AgencyContract> AgencyContracts => Set<Domain.Entities.AgencyContract>();
        public DbSet<Domain.Entities.MedicalService> MedicalServices => Set<Domain.Entities.MedicalService>();
        public DbSet<Domain.Entities.PriceListItem> PriceList => Set<Domain.Entities.PriceListItem>();
        public DbSet<Domain.Entities.Reservation> Reservations => Set<Domain.Entities.Reservation>();
        public DbSet<Domain.Entities.PreContract> PreContracts => Set<Domain.Entities.PreContract>();
        public DbSet<Domain.Entities.Payment> Payments => Set<Domain.Entities.Payment>();
        public DbSet<Domain.Entities.EquipmentStatusLog> EquipmentStatusLogs => Set<Domain.Entities.EquipmentStatusLog>();
        public DbSet<Domain.Entities.EquipmentServiceOrder> EquipmentServiceOrders => Set<Domain.Entities.EquipmentServiceOrder>();
        public DbSet<Domain.Entities.Discount> Discounts => Set<Domain.Entities.Discount>();
        public DbSet<Domain.Entities.DiscountRequest> DiscountRequests => Set<Domain.Entities.DiscountRequest>();
        public DbSet<Domain.Entities.ErrorLog> ErrorLogs => Set<Domain.Entities.ErrorLog>();
        public DbSet<Domain.Entities.RequestLog> RequestLogs => Set<Domain.Entities.RequestLog>();
        public DbSet<Domain.Entities.ThrottleLog> ThrottleLogs => Set<Domain.Entities.ThrottleLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User role relationships
            modelBuilder.Entity<Domain.Entities.User>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.User)
                .HasForeignKey<Domain.Entities.Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Domain.Entities.User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Domain.Entities.Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Domain.Entities.User>()
                .HasOne(u => u.Hospital)
                .WithOne(h => h.User)
                .HasForeignKey<Domain.Entities.Hospital>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Domain.Entities.User>()
                .HasOne(u => u.InsuranceAgency)
                .WithOne(ia => ia.User)
                .HasForeignKey<Domain.Entities.InsuranceAgency>(ia => ia.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: Check constraints will be added in a separate migration after data is fixed
        }
    }
}


