using Domain.Entities;
using Shared.Repository;

namespace Domain.UnitOfWork;

public interface IOrderEventRepository : IRepository<OrderEvent>
{
    ValueTask<IEnumerable<OrderEvent>> GetPendingEvents(
        Guid orderId, CancellationToken token = default);
}