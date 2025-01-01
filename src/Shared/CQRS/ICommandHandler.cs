﻿namespace Shared.CQRS;

public interface ICommandHandler<TCommand>
{
    ValueTask ExecuteAsync(TCommand command);
}