namespace Shared;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public Task<int> FlushAsync(CancellationToken cancellationToken = default);
}