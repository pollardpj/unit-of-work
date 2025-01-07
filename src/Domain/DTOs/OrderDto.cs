using System.Text.Json.Serialization;

namespace Domain.DTOs;

public class OrderDto
{
    public Guid Id { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<OrderEventDto> Events { get; init; }
    public byte[] RowVersion { get; init; }
}