namespace Shared.CQRS;

public interface IQuery<TResult>;


public interface ICacheableQuery<TResult> : IQuery<TResult>
{
    string CacheKey { get; }
}