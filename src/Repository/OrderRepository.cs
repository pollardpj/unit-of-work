using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Repository;

public class OrderRepository(MyAppContext _context) 
    : Repository<Order>(_context), IOrderRepository
{
    public async ValueTask<Order> GetOrderWithEvents(
        Guid reference, CancellationToken token = default)
    {
        return await _context.Set<Order>()
            .Include(u => u.Events)
            .FirstOrDefaultAsync(u => u.Reference == reference, token);
    }

    public IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents(
        DateTime createdBeforeTimestampUtc, CancellationToken token = default)
    {
        return _context.Set<Order>()
            .Where(o => o.Events.Any(e => e.Status == EventStatus.Pending && 
                                          e.CreatedTimestampUtc < createdBeforeTimestampUtc))
            .OrderBy(o => o.Id)
            .Select(o => o.Reference)
            .AsAsyncEnumerable();
    }
}