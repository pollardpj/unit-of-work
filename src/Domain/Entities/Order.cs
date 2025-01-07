namespace Domain.Entities;

public class Order
{
    public Guid Id { get; init; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public ICollection<OrderEvent> Events { get; } = [];
    public byte[] RowVersion { get; set; }
}
