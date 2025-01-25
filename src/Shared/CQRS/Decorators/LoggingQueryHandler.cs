using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class LoggingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated,
    ILogger<LoggingQueryHandler<TQuery, TResult>> _logger) : IQueryHandler<TQuery, TResult>, IDecorator
    where TQuery : IQuery<TResult>
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {QueryHandler} with {Query}",
            TypeUtils.GetInnermostDecoratedName(this),
            JsonSerializer.Serialize(query, JsonHelpers.DefaultOptions));

        try
        {
            var result = await _decorated.ExecuteAsync(query, token);

            _logger.LogDebug("Exited {QueryHandler} with {Result}",
                TypeUtils.GetInnermostDecoratedName(this),
                JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception caught on {QueryHandler}", TypeUtils.GetInnermostDecoratedName(this));
            throw;
        }
    }
}