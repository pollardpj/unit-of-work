using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shared.Json;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class LoggingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated,
    ILogger<LoggingQueryHandler<TQuery, TResult>> _logger) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {QueryHandler} with {Query}",
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()),
            JsonSerializer.Serialize(query, JsonHelpers.DefaultOptions));

        try
        {
            var result = await _decorated.ExecuteAsync(query, token);

            _logger.LogDebug("Exited {QueryHandler} with {Result}",
                TypeUtils.GetUnderlyingTypeName(_decorated.GetType()),
                JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception caught on {QueryHandler}", TypeUtils.GetUnderlyingTypeName(_decorated.GetType()));
            throw;
        }
    }
}