using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Repository;

public class OrderEventRepository(MyAppContext _context) 
    : Repository<OrderEvent>(_context), IOrderEventRepository
{
    public async ValueTask<IEnumerable<OrderEvent>> GetPendingEvents(
        Guid orderId, CancellationToken token = default)
    {
        return await _context.Set<Order>()
            .Where(o => o.Id == orderId)
            .SelectMany(o => o.Events
                .Where(e => e.Status == EventStatus.Pending))
            .OrderBy(e => e.CreatedTimestampUtc)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public override void SetOriginalRowVersion(OrderEvent entity, byte[] rowVersion)
    {
        throw new NotImplementedException();
    }
}