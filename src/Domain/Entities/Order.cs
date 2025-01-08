using Shared.Repository;

namespace Domain.Entities;

public class Order : IEntity
{
    public Guid Id { get; init; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedTimestampUtc { get; set; }
    public string Email { get; set; }
    public ICollection<OrderEvent> Events { get; } = [];
    public byte[] RowVersion { get; set; }
}
