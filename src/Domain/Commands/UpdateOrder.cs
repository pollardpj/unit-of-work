using Shared.CQRS;

namespace Domain.Commands;

public class UpdateOrder : ICacheInvalidatingCommand
{
    public Guid Id { get; set; }
    public required string ProductName { get; init; }
    public required byte[] RowVersion { get; init; }
    public string CacheKey => $"order-{Id}";
}

public class UpdateOrderResult
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public byte[] RowVersion { get; init; }
}