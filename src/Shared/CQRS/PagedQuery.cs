namespace Shared.CQRS;

public abstract class PagedQuery
{
    public string Filter { get; set; }
    public int? Skip { get; set; }
    public int? Top { get; set; }
    public string OrderBy { get; set; }
}
