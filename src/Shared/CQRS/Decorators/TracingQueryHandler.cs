using Shared.Observability;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class TracingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated) : IQueryHandler<TQuery, TResult>, IDecorator
    where TQuery : IQuery<TResult>
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {QueryHandler}", 
            TypeUtils.GetInnermostDecoratedName(this));

        return await _decorated.ExecuteAsync(query, token);
    }
}