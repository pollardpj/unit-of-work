namespace Shared.Repository;

public interface IRepository<T, in TId> where T : class, IEntity<TId>
{
    IQueryable<T> GetIQueryable();
    ValueTask<List<T>> GetAllAsync();
    ValueTask<T> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(T entity, byte[] rowVersion);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}