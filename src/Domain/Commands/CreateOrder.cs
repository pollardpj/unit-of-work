namespace Domain.Commands;

public class CreateOrder
{
    public required string ProductName { get; init; }
}

public class CreateOrderResult
{
    public Guid Reference { get; init; }
    public decimal Price { get; init; }
}
