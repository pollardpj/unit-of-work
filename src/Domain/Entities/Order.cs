namespace Domain.Entities;

public class Order
{
    public int Id { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public int UserId { get; init; }
    public User User { get; init; }
}