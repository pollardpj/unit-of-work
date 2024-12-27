using Domain.Entities;
using Shared;

namespace Repository;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetOrderWithEvents(Guid reference);
    IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents();
}