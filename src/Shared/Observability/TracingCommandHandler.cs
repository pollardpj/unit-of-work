using Shared.CQRS;

namespace Shared.Observability;

public class TracingCommandHandler<TCommand>(
ICommandHandler<TCommand> _decorated) : ICommandHandler<TCommand>
{
    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("CommandHandler: {TCommand}", typeof(TCommand).Name);

        await _decorated.ExecuteAsync(command, token);
    }
}

public class TracingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> _decorated) : ICommandHandler<TCommand, TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("CommandHandler: {TCommand}", typeof(TCommand).Name);

        return await _decorated.ExecuteAsync(command, token);
    }
}
