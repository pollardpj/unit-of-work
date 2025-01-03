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
        Guid orderReference, CancellationToken token = default)
    {
        return await _context.Set<Order>()
            .Where(o => o.Reference == orderReference)
            .SelectMany(o => o.Events
                .Where(e => e.Status == EventStatus.Pending))
            .OrderBy(e => e.CreatedTimestampUtc)
            .AsNoTracking()
            .ToListAsync(token);
    }
}