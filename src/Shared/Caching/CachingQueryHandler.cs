using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.CQRS;
using Shared.Json;
using Shared.Observability;
using Shared.Utils;

namespace Shared.Caching;

public class CachingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated,
    HybridCache _cache,
    ILogger<LoggingQueryHandler<TQuery, TResult>> _logger) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        var queryAsJson = JsonSerializer.Serialize(query, JsonHelpers.DefaultOptions);
        var queryAsHash = HashUtils.GetHash(queryAsJson);
            
        var result = await _cache.GetOrCreateAsync(
            $"order-{queryAsHash}",
            async cancellationToken =>
            {
                _logger.LogDebug("Cache miss on {QueryHandler} with {Query}",
                     TypeUtils.GetUnderlyingTypeName(_decorated.GetType()), queryAsJson);
                
                return await _decorated.ExecuteAsync(query, cancellationToken);
            },
            cancellationToken: token);
        
        return result;
    }
}