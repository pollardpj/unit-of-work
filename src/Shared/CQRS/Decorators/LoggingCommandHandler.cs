using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shared.Utils;

namespace Shared.CQRS.Decorators;

public class LoggingCommandHandler<TCommand>(
ICommandHandler<TCommand> _decorated,
ILogger<LoggingCommandHandler<TCommand>> _logger) : ICommandHandler<TCommand>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {CommandHandler} with {Command}",
            TypeUtils.GetInnermostDecoratedName(this),
            JsonSerializer.Serialize(command, JsonHelpers.DefaultOptions));

        await _decorated.ExecuteAsync(command, token);
    }
}

public class LoggingCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> _decorated,
    ILogger<LoggingCommandHandler<TCommand, TResult>> _logger) : ICommandHandler<TCommand, TResult>, IDecorator
{
    public object Decorated => _decorated;

    public async ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken token = default)
    {
        _logger.LogDebug("Entered {CommandHandler} with {Command}", 
            TypeUtils.GetInnermostDecoratedName(this),
            JsonSerializer.Serialize(command, JsonHelpers.DefaultOptions));

        try
        {
            var result = await _decorated.ExecuteAsync(command, token);

            _logger.LogDebug("Exited {CommandHandler} with {Result}",
                TypeUtils.GetInnermostDecoratedName(this),
                JsonSerializer.Serialize(result, JsonHelpers.DefaultOptions));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception caught on {CommandHandler}", TypeUtils.GetInnermostDecoratedName(this));
            throw;
        }
    }
}
