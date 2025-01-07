namespace Shared.Repository;

public interface IRepository<T> where T : class, IEntity
{
    IQueryable<T> GetIQueryable();
    ValueTask<List<T>> GetAllAsync();
    ValueTask<T> GetByIdAsync(Guid id);
    void SetOriginalRowVersion(T entity, byte[] rowVersion);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}