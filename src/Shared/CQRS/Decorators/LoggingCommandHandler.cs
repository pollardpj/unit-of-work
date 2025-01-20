using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shared.Json;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class LoggingCommandHandler<TCommand>(
ICommandHandler<TCommand> _decorated,
ILogger<LoggingCommandHandler<TCommand>> _logger) : ICommandHandler<TCommand>
{
    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {CommandHandler} with {Command}",
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()),
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
            TypeUtils.GetUnderlyingTypeName(_decorated.GetType()),
            JsonSerializer.Serialize(command, JsonHelpers.DefaultOptions));

        try
        {
            var result = await _decorated.ExecuteAsync(command, token);

            _logger.LogDebug("Exited {CommandHandler} with {Result}",
                TypeUtils.GetUnderlyingTypeName(_decorated.GetType()),
                JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception caught on {CommandHandler}", _decorated.GetType().Name);
            throw;
        }
    }
}
