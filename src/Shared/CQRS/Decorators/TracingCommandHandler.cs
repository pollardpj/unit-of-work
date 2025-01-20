using Shared.Observability;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class TracingCommandHandler<TCommand>(
ICommandHandler<TCommand> _decorated) : ICommandHandler<TCommand>
{
    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {CommandHandler}", 
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()));

        await _decorated.ExecuteAsync(command, token);
    }
}

public class TracingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> _decorated) : ICommandHandler<TCommand, TResult>
{
    public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        using var _ = TracingHelpers.StartActivity("Call {CommandHandler}", 
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()));

        return await _decorated.ExecuteAsync(command, token);
    }
}
