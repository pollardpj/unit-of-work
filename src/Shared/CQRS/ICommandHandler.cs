namespace Shared.CQRS;

public interface ICommandHandler<in TCommand>
{
    ValueTask ExecuteAsync(TCommand command, CancellationToken token = default);
}

public interface ICommandHandler<in TCommand, TResult>
{
    ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default);
}