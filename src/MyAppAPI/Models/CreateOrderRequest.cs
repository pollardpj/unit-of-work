namespace MyAppAPI.Models;

public class CreateOrderRequest
{
    public Guid Reference { get; set; }
    public string ProductName { get; set; }
}