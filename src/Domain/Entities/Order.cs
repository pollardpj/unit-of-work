namespace Domain.Entities;

public class Order
{
    public int Id { get; init; }
    public Guid Reference { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public User User { get; init; }
    public ICollection<OrderEvent> Events { get; } = new List<OrderEvent>();
}
