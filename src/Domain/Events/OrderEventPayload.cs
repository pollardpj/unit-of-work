namespace Domain.Events;

public class OrderEventPayload
{
    public Guid Reference { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
}