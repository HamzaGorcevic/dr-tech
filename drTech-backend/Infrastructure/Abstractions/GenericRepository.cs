using drTech_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace drTech_backend.Infrastructure.Abstractions
{
    public enum DatabaseProvider
    {
        PostgreSQL,
        MongoDB,
        Neo4j
    }

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default);
    }

    public interface IGenericUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    // Generic repository implementations for each database
    public class PostgreSqlRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Infrastructure.AppDbContext _db;
        private readonly Microsoft.EntityFrameworkCore.DbSet<T> _set;

        public PostgreSqlRepository(Infrastructure.AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => 
            await _set.FindAsync(new object?[] { id }, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) => 
            await _set.AddAsync(entity, ct);

        public void Update(T entity) => _set.Update(entity);

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            Update(entity);
            await Task.CompletedTask;
        }

        public void Delete(T entity) => _set.Remove(entity);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity != null) Delete(entity);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default) => 
            await _set.AsNoTracking().ToListAsync(ct);

        public async Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default) => 
            await Task.FromResult(_set.AsNoTracking().Where(predicate).ToList());
    }

    public class MongoDbRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Mongo.IMongoRepository<T> _mongoRepo;

        public MongoDbRepository(Mongo.IMongoRepository<T> mongoRepo)
        {
            _mongoRepo = mongoRepo;
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => 
            await _mongoRepo.GetByIdAsync(id, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) => 
            await _mongoRepo.AddAsync(entity, ct);

        public async Task UpdateAsync(T entity, CancellationToken ct = default) => 
            await _mongoRepo.UpdateAsync(entity, ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default) => 
            await _mongoRepo.DeleteAsync(id, ct);

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default) => 
            await _mongoRepo.GetAllAsync(ct);

        public async Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default) => 
            await Task.FromResult((await GetAllAsync(ct)).Where(predicate).ToList());
    }

    public class Neo4jRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Neo4j.INeo4jRepository _neo4jRepo;
        private readonly string _label;

        public Neo4jRepository(Neo4j.INeo4jRepository neo4jRepo, string label)
        {
            _neo4jRepo = neo4jRepo;
            _label = label;
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => 
            await _neo4jRepo.GetByIdAsync<T>(id, _label, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) => 
            await _neo4jRepo.AddAsync(entity, _label, ct);

        public async Task UpdateAsync(T entity, CancellationToken ct = default) => 
            await _neo4jRepo.UpdateAsync(entity, _label, ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default) => 
            await _neo4jRepo.DeleteAsync(id, _label, ct);

        public async Task<List<T>> GetAllAsync(CancellationToken ct = default) => 
            await _neo4jRepo.GetAllAsync<T>(_label, ct);

        public async Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default) => 
            await Task.FromResult((await GetAllAsync(ct)).Where(predicate).ToList());
    }

    public class PostgreSqlUnitOfWork : IGenericUnitOfWork
    {
        private readonly Infrastructure.AppDbContext _db;

        public PostgreSqlUnitOfWork(Infrastructure.AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => 
            await _db.SaveChangesAsync(ct);
    }

    public class MongoDbUnitOfWork : IGenericUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => 
            await Task.FromResult(1); // MongoDB operations are immediate
    }

    public class Neo4jUnitOfWork : IGenericUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => 
            await Task.FromResult(1); // Neo4j operations are immediate
    }
}
