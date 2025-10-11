using Microsoft.EntityFrameworkCore;

namespace drTech_backend.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Infrastructure.AppDbContext _db;
        private readonly DbSet<T> _set;

        public Repository(Infrastructure.AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _set.FindAsync(new object?[] { id }, ct);
        public async Task AddAsync(T entity, CancellationToken ct = default) => await _set.AddAsync(entity, ct);
        public void Update(T entity) => _set.Update(entity);
        public void Remove(T entity) => _set.Remove(entity);
        public IQueryable<T> Query() => _set.AsQueryable();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly Infrastructure.AppDbContext _db;
        public UnitOfWork(Infrastructure.AppDbContext db) { _db = db; }
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}


