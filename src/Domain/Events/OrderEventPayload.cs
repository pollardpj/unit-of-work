using Domain.Enums;

namespace Domain.Events;

public class OrderEventPayload
{
    public required Guid EventId { get; init; }
    public required Guid OrderId { get; init; }
    public required OrderEventType Type { get; init; }
    public required string ProductName { get; init; }
    public required decimal Price { get; init; }
}