namespace MyAppAPI.Models;

public class UpdateOrderRequest
{
    public string ProductName { get; init; }
    public string RowVersion { get; init; }
}