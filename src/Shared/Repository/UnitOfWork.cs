using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    private bool _disposed;

    public async ValueTask<int> FlushAsync(CancellationToken token = default)
    {
        return await context.SaveChangesAsync(token);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (context != null) await context.DisposeAsync();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}