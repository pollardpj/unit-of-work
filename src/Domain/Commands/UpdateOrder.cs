namespace Domain.Commands;

public class UpdateOrder
{
    public Guid Id { get; set; }
    public required string ProductName { get; init; }
    public required byte[] RowVersion { get; init; }
}

public class UpdateOrderResult
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public byte[] RowVersion { get; init; }
}