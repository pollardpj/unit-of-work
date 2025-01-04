namespace Shared.Exceptions;

public abstract class BadRequestException
    (string message, Exception innerException) : Exception(message, innerException)
{
    public abstract object Details { get; }
}
