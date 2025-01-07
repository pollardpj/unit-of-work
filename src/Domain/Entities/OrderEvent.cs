using Domain.Enums;

namespace Domain.Entities;

public class OrderEvent
{
    public Guid Id { get; init; }
    public OrderEventType Type { get; init; }
    public DateTime CreatedTimestampUtc { get; init; }
    public string Payload { get; init; }
    public EventStatus Status { get; set; }
}