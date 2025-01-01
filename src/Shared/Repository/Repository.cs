﻿using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    protected Repository(DbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async ValueTask<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async ValueTask<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
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