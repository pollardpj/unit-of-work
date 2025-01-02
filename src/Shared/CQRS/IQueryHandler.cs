namespace Shared.CQRS;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    ValueTask<TResult> ExecuteAsync(TQuery query);
}
