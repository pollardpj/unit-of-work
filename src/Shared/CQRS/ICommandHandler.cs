namespace Shared.CQRS;

public interface ICommandHandler<TCommand>
{
    ValueTask ExecuteAsync(TCommand command, CancellationToken token = default);
}

public interface ICommandHandler<TCommand, TResult>
{
    ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default);
}