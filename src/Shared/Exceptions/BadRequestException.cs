namespace Shared.Exceptions;

public abstract class BadRequestException
    (string message, Exception innerException) : Exception(message, innerException)
{
    public abstract object Errors { get; }

    public object Details => new
    {
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        Title = "One or more validation errors occurred.",
        Status = 400,
        Errors
    };
}
