﻿namespace Shared.Repository;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public ValueTask<int> FlushAsync(CancellationToken token = default);
}