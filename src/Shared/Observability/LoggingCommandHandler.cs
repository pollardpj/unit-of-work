using Microsoft.Extensions.Logging;
using Shared.CQRS;
using System.Text.Json;

namespace Shared.Observability
{
    public class LoggingCommandHandler<TCommand>(
    ICommandHandler<TCommand> _decorated,
    ILogger<LoggingCommandHandler<TCommand>> _logger) : ICommandHandler<TCommand>
    {
        public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            _logger.LogDebug("Entered {CommandHandler} with {Command}",
                _decorated.GetType().Name,
                JsonSerializer.Serialize(command, JsonHelpers.DefaultOptions));

            await _decorated.ExecuteAsync(command, token);
        }
    }

    public class LoggingCommandHandler<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> _decorated,
        ILogger<LoggingCommandHandler<TCommand, TResult>> _logger) : ICommandHandler<TCommand, TResult>
    {
        public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            _logger.LogDebug("Entered {CommandHandler} with {Command}", 
                _decorated.GetType().Name,
                JsonSerializer.Serialize(command, JsonHelpers.DefaultOptions));

            var result = await _decorated.ExecuteAsync(command, token);

            _logger.LogDebug("Exited {CommandHandler} with {Result}",
                _decorated.GetType().Name,
                JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

            return result;
        }
    }
}
