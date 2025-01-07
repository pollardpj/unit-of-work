namespace Shared.Exceptions;

public class ConflictException
    (string message, Exception innerException) : Exception(message, innerException)
{
}
