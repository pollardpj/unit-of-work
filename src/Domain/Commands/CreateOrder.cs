namespace Domain.Commands
{
    public class CreateOrder
    {
        public required Guid Reference { get; init; }
        public required string ProductName { get; init; }
    }

    public class CreateOrderResult
    {
        public decimal Price { get; init; }
    }
}
