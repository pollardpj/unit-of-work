using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public class UnitOfWork(DbContext _context) : IUnitOfWork
{
    private bool _disposed;

    public async ValueTask<int> FlushAsync(CancellationToken token = default)
    {
        return await _context.SaveChangesAsync(token);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
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
        if (_context != null) await _context.DisposeAsync();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}