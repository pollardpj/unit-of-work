using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class CachingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated,
    HybridCache _cache,
    ILogger<CachingQueryHandler<TQuery, TResult>> _logger) : IQueryHandler<TQuery, TResult>, IDecorator
    where TQuery : IQuery<TResult>
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        if (query is not ICacheableQuery<TResult> cacheableQuery)
        {
            return await _decorated.ExecuteAsync(query, token);
        }
        
        var queryHandlerName = TypeUtils.GetInnermostDecoratedName(this);
        var queryAsJson = JsonSerializer.Serialize(cacheableQuery, JsonHelpers.DefaultOptions);
            
        var result = await _cache.GetOrCreateAsync(
            cacheableQuery.CacheKey,
            async cancellationToken =>
            {
                _logger.LogDebug("Cache miss on {QueryHandler} with {Query}", queryHandlerName, queryAsJson);
                
                return await _decorated.ExecuteAsync(query, cancellationToken);
            },
            cancellationToken: token);
        
        return result;
    }
}