using Microsoft.Extensions.Logging;
using Shared.CQRS;

namespace Shared.Observability
{
    public class TracingCommandHandler<TCommand>(
    ICommandHandler<TCommand> _decorated,
    ILogger<TracingCommandHandler<TCommand>> _logger) : ICommandHandler<TCommand>
    {
        public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            using var _ = TracingHelpers.StartActivity("CommandHandler: {TCommand}", typeof(TCommand).Name);

            await _decorated.ExecuteAsync(command, token);
        }
    }

    public class TracingCommandHandler<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> _decorated,
        ILogger<TracingCommandHandler<TCommand, TResult>> _logger) : ICommandHandler<TCommand, TResult>
    {
        public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            using var _ = TracingHelpers.StartActivity("CommandHandler: {TCommand} => {TResult}", typeof(TCommand).Name, typeof(TResult).Name);

            return await _decorated.ExecuteAsync(command, token);
        }
    }
}
