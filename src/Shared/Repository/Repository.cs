using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public abstract class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    protected Repository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> GetIQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async ValueTask<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async ValueTask<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id);
    }

    public void SetOriginalRowVersion(T entity, byte[] rowVersion)
    {
        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = rowVersion;
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}