namespace Shared.Exceptions;

public class PagedQueryException(string message, Exception innerException)
    : BadRequestException(message, innerException)
{
    public override object Details => new
    {
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        Title = "One or more validation errors occurred.",
        Status = 400,
        Errors = new { Query = new List<string> { Message } }
    };
}
