namespace Domain.DTOs;

public class OrderDto
{
    public Guid Reference { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public IEnumerable<OrderEventDto> Events { get; set; }
}