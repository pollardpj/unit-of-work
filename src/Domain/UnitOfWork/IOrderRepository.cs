using Domain.Entities;
using Shared.Repository;

namespace Domain.UnitOfWork;

public interface IOrderRepository : IRepository<Order>
{
    ValueTask<Order> GetOrderWithEvents(
        Guid id, CancellationToken token = default);

    IAsyncEnumerable<Guid> GetOrderIdsWithPendingEvents(
        DateTime createdBeforeTimestampUtc, CancellationToken token = default);
}