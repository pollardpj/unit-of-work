using Microsoft.Extensions.Caching.Hybrid;

namespace Shared.CQRS.Decorators;

public class CacheInvalidatingCommandHandler<TCommand>(
    ICommandHandler<TCommand> _decorated,
    HybridCache _cache) : ICommandHandler<TCommand>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        await _decorated.ExecuteAsync(command, token);

        if (command is ICacheInvalidatingCommand cacheInvalidatingCommand)
        {
            await _cache.RemoveAsync(cacheInvalidatingCommand.CacheKey, token);
        }
    }
}

public class CacheInvalidatingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> _decorated,
    HybridCache _cache) : ICommandHandler<TCommand, TResult>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        var result = await _decorated.ExecuteAsync(command, token);
        
        if (command is ICacheInvalidatingCommand cacheInvalidatingCommand)
        {
            await _cache.RemoveAsync(cacheInvalidatingCommand.CacheKey, token);
        }

        return result;
    }
}
