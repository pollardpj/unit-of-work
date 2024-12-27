using Domain.Enums;

namespace Domain.Entities;

public class OrderEvent
{
    public int Id { get; set; }
    public Guid Reference { get; set; }
    public OrderEventType Type { get; set; }
    public DateTime CreatedTimestampUtc { get; set; }
    public string Payload { get; set; }
    public EventStatus Status { get; set; }
}