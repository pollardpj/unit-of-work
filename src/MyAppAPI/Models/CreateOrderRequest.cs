namespace MyAppAPI.Models;

public class CreateOrderRequest
{
    public Guid Reference { get; init; }
    public string ProductName { get; init; }
}