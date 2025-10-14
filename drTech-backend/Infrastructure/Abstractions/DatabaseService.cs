using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Infrastructure.Abstractions
{
    public static class DatabaseFactory
    {
        public static IGenericRepository<T> CreateRepository<T>(DatabaseProvider provider, IServiceProvider serviceProvider) where T : class
        {
            return provider switch
            {
                DatabaseProvider.PostgreSQL => new PostgreSqlRepository<T>(serviceProvider.GetRequiredService<Infrastructure.AppDbContext>()),
                DatabaseProvider.MongoDB => new MongoDbRepository<T>(serviceProvider.GetRequiredService<Mongo.IMongoRepository<T>>()),
                DatabaseProvider.Neo4j => new Neo4jRepository<T>(serviceProvider.GetRequiredService<Neo4j.INeo4jRepository>(), GetLabel<T>()),
                _ => throw new ArgumentException($"Unsupported database provider: {provider}")
            };
        }

        public static IGenericUnitOfWork CreateUnitOfWork(DatabaseProvider provider, IServiceProvider serviceProvider)
        {
            return provider switch
            {
                DatabaseProvider.PostgreSQL => new PostgreSqlUnitOfWork(serviceProvider.GetRequiredService<Infrastructure.AppDbContext>()),
                DatabaseProvider.MongoDB => new MongoDbUnitOfWork(),
                DatabaseProvider.Neo4j => new Neo4jUnitOfWork(),
                _ => throw new ArgumentException($"Unsupported database provider: {provider}")
            };
        }

        private static string GetLabel<T>() where T : class
        {
            return typeof(T).Name switch
            {
                nameof(Domain.Entities.Hospital) => "Hospital",
                nameof(Domain.Entities.Department) => "Department",
                nameof(Domain.Entities.Doctor) => "Doctor",
                nameof(Domain.Entities.Patient) => "Patient",
                nameof(Domain.Entities.Equipment) => "Equipment",
                nameof(Domain.Entities.Appointment) => "Appointment",
                nameof(Domain.Entities.Reservation) => "Reservation",
                nameof(Domain.Entities.InsuranceAgency) => "InsuranceAgency",
                nameof(Domain.Entities.AgencyContract) => "AgencyContract",
                nameof(Domain.Entities.PreContract) => "PreContract",
                nameof(Domain.Entities.EquipmentStatusLog) => "EquipmentStatusLog",
                nameof(Domain.Entities.EquipmentServiceOrder) => "EquipmentServiceOrder",
                nameof(Domain.Entities.MedicalService) => "MedicalService",
                nameof(Domain.Entities.PriceListItem) => "PriceListItem",
                nameof(Domain.Entities.Payment) => "Payment",
                nameof(Domain.Entities.User) => "User",
                nameof(Domain.Entities.RefreshToken) => "RefreshToken",
                nameof(Domain.Entities.AuditLog) => "AuditLog",
                nameof(Domain.Entities.RequestLog) => "RequestLog",
                nameof(Domain.Entities.ErrorLog) => "ErrorLog",
                nameof(Domain.Entities.ThrottleLog) => "ThrottleLog",
                nameof(Domain.Entities.Discount) => "Discount",
                nameof(Domain.Entities.DiscountRequest) => "DiscountRequest",
                _ => typeof(T).Name
            };
        }
    }

    public interface IDatabaseService<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public class DatabaseService<T> : IDatabaseService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IGenericUnitOfWork _unitOfWork;

        public DatabaseService(DatabaseProvider provider, IServiceProvider serviceProvider)
        {
            _repository = DatabaseFactory.CreateRepository<T>(provider, serviceProvider);
            _unitOfWork = DatabaseFactory.CreateUnitOfWork(provider, serviceProvider);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => 
            await _repository.GetByIdAsync(id, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) => 
            await _repository.AddAsync(entity, ct);

        public async Task UpdateAsync(T entity, CancellationToken ct = default) => 
            await _repository.UpdateAsync(entity, ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default) => 
            await _repository.DeleteAsync(id, ct);

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default) => 
            await _repository.GetAllAsync(ct);

        public async Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default) => 
            await _repository.FindAsync(predicate, ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => 
            await _unitOfWork.SaveChangesAsync(ct);
    }
}
