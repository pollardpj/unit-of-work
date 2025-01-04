namespace Shared.Repository;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetIQueryable();
    ValueTask<List<T>> GetAllAsync();
    ValueTask<T> GetByIdAsync(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}