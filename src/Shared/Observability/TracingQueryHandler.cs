using Shared.CQRS;

namespace Shared.Observability;

public class TracingQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> _decorated) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("QueryHandler: {TQuery}", typeof(TQuery).Name);

        return await _decorated.ExecuteAsync(query, token);
    }
}