using Domain.Entities;
using Shared.Repository;

namespace Domain.UnitOfWork;

public interface IOrderRepository : IRepository<Order>
{
    ValueTask<Order> GetOrderWithEvents(Guid reference);
    IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents();
}