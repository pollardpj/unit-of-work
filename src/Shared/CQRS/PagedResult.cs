namespace Shared.CQRS;

public abstract class PagedResult<TPagedItem>
{
    public int TotalCount { get; set; }
    public IEnumerable<TPagedItem> Items { get; set; }
}
