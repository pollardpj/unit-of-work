using Domain.Entities;
using Shared;

namespace Domain.UnitOfWork;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetOrderWithEvents(Guid reference);
    IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents();
}