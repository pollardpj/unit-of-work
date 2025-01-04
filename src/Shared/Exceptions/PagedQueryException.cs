namespace Shared.Exceptions;

public class PagedQueryException
    (string message, Exception innerException) : BadRequestException(message, innerException)
{
    public override object Errors => new { Query = new List<string> { Message } };
}
