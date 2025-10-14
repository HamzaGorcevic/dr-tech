using MongoDB.Driver;

namespace drTech_backend.Infrastructure.Mongo
{
    public interface IMongoRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task<List<T>> FindAsync(FilterDefinition<T> filter, CancellationToken ct = default);
    }

    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoContext context)
        {
            var collectionName = GetCollectionName(typeof(T));
            _collection = context.Database.GetCollection<T>(collectionName);
        }

        private static string GetCollectionName(Type entityType)
        {
            return entityType.Name switch
            {
                nameof(Domain.Entities.Hospital) => "Hospitals",
                nameof(Domain.Entities.Department) => "Departments",
                nameof(Domain.Entities.Doctor) => "Doctors",
                nameof(Domain.Entities.Patient) => "Patients",
                nameof(Domain.Entities.Equipment) => "Equipment",
                nameof(Domain.Entities.Appointment) => "Appointments",
                nameof(Domain.Entities.Reservation) => "Reservations",
                nameof(Domain.Entities.InsuranceAgency) => "InsuranceAgencies",
                nameof(Domain.Entities.AgencyContract) => "AgencyContracts",
                nameof(Domain.Entities.MedicalService) => "MedicalServices",
                nameof(Domain.Entities.PriceListItem) => "PriceListItems",
                nameof(Domain.Entities.Payment) => "Payments",
                nameof(Domain.Entities.User) => "Users",
                nameof(Domain.Entities.RefreshToken) => "RefreshTokens",
                nameof(Domain.Entities.AuditLog) => "AuditLogs",
                nameof(Domain.Entities.Discount) => "Discounts",
                nameof(Domain.Entities.DiscountRequest) => "DiscountRequests",
                nameof(Domain.Entities.ErrorLog) => "ErrorLogs",
                nameof(Domain.Entities.RequestLog) => "RequestLogs",
                nameof(Domain.Entities.ThrottleLog) => "ThrottleLogs",
                nameof(Domain.Entities.EquipmentStatusLog) => "EquipmentStatusLogs",
                nameof(Domain.Entities.EquipmentServiceOrder) => "EquipmentServiceOrders",
                nameof(Domain.Entities.PreContract) => "PreContracts",
                _ => entityType.Name + "s"
            };
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: ct);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", GetId(entity));
            await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter, ct);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _collection.Find(_ => true).ToListAsync(ct);
        }

        public async Task<List<T>> FindAsync(FilterDefinition<T> filter, CancellationToken ct = default)
        {
            return await _collection.Find(filter).ToListAsync(ct);
        }

        private static Guid GetId(T entity)
        {
            return entity.GetType().GetProperty("Id")?.GetValue(entity) as Guid? ?? Guid.Empty;
        }
    }

    // Specific repositories for each entity
    public interface IMongoHospitalRepository : IMongoRepository<MongoHospital> { }
    public interface IMongoPatientRepository : IMongoRepository<MongoPatient> { }
    public interface IMongoDoctorRepository : IMongoRepository<MongoDoctor> { }
    public interface IMongoDepartmentRepository : IMongoRepository<MongoDepartment> { }
    public interface IMongoEquipmentRepository : IMongoRepository<MongoEquipment> { }
    public interface IMongoReservationRepository : IMongoRepository<MongoReservation> { }
    public interface IMongoAgencyRepository : IMongoRepository<MongoInsuranceAgency> { }
    public interface IMongoContractRepository : IMongoRepository<MongoAgencyContract> { }
    public interface IMongoServiceRepository : IMongoRepository<MongoMedicalService> { }
    public interface IMongoPriceListRepository : IMongoRepository<MongoPriceListItem> { }
    public interface IMongoPaymentRepository : IMongoRepository<MongoPayment> { }
    public interface IMongoUserRepository : IMongoRepository<MongoUser> { }

    public class MongoHospitalRepository : MongoRepository<MongoHospital>, IMongoHospitalRepository
    {
        public MongoHospitalRepository(IMongoContext context) : base(context) { }
    }

    public class MongoPatientRepository : MongoRepository<MongoPatient>, IMongoPatientRepository
    {
        public MongoPatientRepository(IMongoContext context) : base(context) { }
    }

    public class MongoDoctorRepository : MongoRepository<MongoDoctor>, IMongoDoctorRepository
    {
        public MongoDoctorRepository(IMongoContext context) : base(context) { }
    }

    public class MongoDepartmentRepository : MongoRepository<MongoDepartment>, IMongoDepartmentRepository
    {
        public MongoDepartmentRepository(IMongoContext context) : base(context) { }
    }

    public class MongoEquipmentRepository : MongoRepository<MongoEquipment>, IMongoEquipmentRepository
    {
        public MongoEquipmentRepository(IMongoContext context) : base(context) { }
    }

    public class MongoReservationRepository : MongoRepository<MongoReservation>, IMongoReservationRepository
    {
        public MongoReservationRepository(IMongoContext context) : base(context) { }
    }

    public class MongoAgencyRepository : MongoRepository<MongoInsuranceAgency>, IMongoAgencyRepository
    {
        public MongoAgencyRepository(IMongoContext context) : base(context) { }
    }

    public class MongoContractRepository : MongoRepository<MongoAgencyContract>, IMongoContractRepository
    {
        public MongoContractRepository(IMongoContext context) : base(context) { }
    }

    public class MongoServiceRepository : MongoRepository<MongoMedicalService>, IMongoServiceRepository
    {
        public MongoServiceRepository(IMongoContext context) : base(context) { }
    }

    public class MongoPriceListRepository : MongoRepository<MongoPriceListItem>, IMongoPriceListRepository
    {
        public MongoPriceListRepository(IMongoContext context) : base(context) { }
    }

    public class MongoPaymentRepository : MongoRepository<MongoPayment>, IMongoPaymentRepository
    {
        public MongoPaymentRepository(IMongoContext context) : base(context) { }
    }

    public class MongoUserRepository : MongoRepository<MongoUser>, IMongoUserRepository
    {
        public MongoUserRepository(IMongoContext context) : base(context) { }
    }
}
