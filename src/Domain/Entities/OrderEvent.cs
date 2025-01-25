using Domain.Enums;
using Shared.Repository;

namespace Domain.Entities;

public class OrderEvent : IEntity<Guid>
{
    public Guid Id { get; init; }
    public OrderEventType Type { get; init; }
    public DateTime CreatedTimestampUtc { get; init; }
    public string Payload { get; init; }
    public EventStatus Status { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public byte[] RowVersion { get; set; }
}