using Domain.Enums;

namespace Domain.Events;

public class OrderEventPayload
{
    public required Guid Id { get; init; }
    public required OrderEventType Type { get; init; }
    public required string ProductName { get; init; }
    public required decimal Price { get; init; }
}