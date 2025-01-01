using Microsoft.EntityFrameworkCore;

namespace Shared;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    private bool _disposed;

    public async Task<int> FlushAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
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