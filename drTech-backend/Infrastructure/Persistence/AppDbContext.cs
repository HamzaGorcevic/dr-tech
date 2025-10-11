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
    }
}


