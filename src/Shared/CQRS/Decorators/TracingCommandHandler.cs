using Shared.Observability;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class TracingCommandHandler<TCommand>(
ICommandHandler<TCommand> _decorated) : ICommandHandler<TCommand>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {CommandHandler}", 
            TypeUtils.GetInnermostDecoratedName(this));

        await _decorated.ExecuteAsync(command, token);
    }
}

public class TracingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> _decorated) : ICommandHandler<TCommand, TResult>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {CommandHandler}", 
            TypeUtils.GetInnermostDecoratedName(this));

        return await _decorated.ExecuteAsync(command, token);
    }
}
