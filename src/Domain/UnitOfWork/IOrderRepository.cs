using Domain.Entities;
using Shared.Repository;

namespace Domain.UnitOfWork;

public interface IOrderRepository : IRepository<Order>
{
    ValueTask<Order> GetOrderWithEvents(
        Guid reference, CancellationToken token = default);

    IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents(
        DateTime createdBeforeTimestampUtc, CancellationToken token = default);
}