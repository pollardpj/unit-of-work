namespace Shared.CQRS;

public abstract class PagedQuery
{
    public string Filter { get; init; }
    public int? Skip { get; init; }
    public int? Top { get; init; }
    public string OrderBy { get; init; }
}
