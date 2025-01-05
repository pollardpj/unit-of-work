using Microsoft.Extensions.Logging;
using Shared.CQRS;
using System.Text.Json;

namespace Shared.Observability;

public class LoggingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated,
    ILogger<LoggingQueryHandler<TQuery, TResult>> _logger) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {QueryHandler} with {Query}",
            _decorated.GetType().Name,
            JsonSerializer.Serialize(query, JsonHelpers.DefaultOptions));

        var result = await _decorated.ExecuteAsync(query, token);

        _logger.LogDebug("Exited {QueryHandler} with {Result}",
            _decorated.GetType().Name,
            JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

        return result;
    }
}