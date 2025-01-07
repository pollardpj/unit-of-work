namespace Shared.Exceptions;

public class BadRequestException
    (string message, Exception innerException) : Exception(message, innerException)
{
}
