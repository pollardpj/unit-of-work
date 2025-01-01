namespace MyAppAPI.Models;

public class OrderRequest
{
    public Guid Reference { get; set; }
    public string ProductName { get; set; }
}