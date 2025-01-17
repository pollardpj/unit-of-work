namespace Shared.CQRS;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    ValueTask<TResult> ExecuteAsync(TQuery query, CancellationToken token = default);
}
