using Domain.Enums;

namespace Domain.DTOs;

public class OrderEventDto
{
    public Guid Reference { get; set; }
    public OrderEventType Type { get; set; }
    public DateTime CreatedTimestampUtc { get; set; }
    public string Payload { get; set; }
    public EventStatus Status { get; set; }
}