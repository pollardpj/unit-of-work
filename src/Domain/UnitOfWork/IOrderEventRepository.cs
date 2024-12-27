using Domain.Entities;
using Shared;

namespace Domain.UnitOfWork;

public interface IOrderEventRepository : IRepository<OrderEvent>
{
    Task<IEnumerable<OrderEvent>> GetPendingEvents(Guid orderReference);
}