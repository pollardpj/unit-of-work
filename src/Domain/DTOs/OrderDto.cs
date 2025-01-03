namespace Domain.DTOs;

public class OrderDto
{
    public Guid Reference { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public IEnumerable<OrderEventDto> Events { get; init; }
}