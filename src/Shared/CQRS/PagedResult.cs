namespace Shared.CQRS;

public abstract class PagedResult<TPagedItem>
{
    public required int TotalCount { get; init; }
    public required IEnumerable<TPagedItem> Items { get; init; }
}
