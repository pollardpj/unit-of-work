using Domain.Enums;

namespace Domain.DTOs;

public class OrderEventDto
{
    public Guid Reference { get; init; }
    public OrderEventType Type { get; init; }
    public DateTime CreatedTimestampUtc { get; init; }
    public string Payload { get; init; }
    public EventStatus Status { get; init; }
}