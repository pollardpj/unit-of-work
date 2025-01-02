namespace Domain.Exceptions;

public class PagedQueryException : Exception
{
    public PagedQueryException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public object Details => new
    {
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        Title = "One or more validation errors occurred.",
        Status = 400,
        Errors = new { Query = new List<string> { Message } }
    };
}
