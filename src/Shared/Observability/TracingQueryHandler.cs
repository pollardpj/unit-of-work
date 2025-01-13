using Shared.CQRS;
using Shared.Utils;

namespace Shared.Observability;

public class TracingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {QueryHandler}", 
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()));

        return await _decorated.ExecuteAsync(query, token);
    }
}