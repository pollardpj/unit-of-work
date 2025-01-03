namespace Shared.CQRS;

public abstract class PagedResult<TPagedItem>
{
    public int TotalCount { get; init; }
    public IEnumerable<TPagedItem> Items { get; init; }
}
