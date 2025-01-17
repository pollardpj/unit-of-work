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
        Guid id, CancellationToken token = default)
    {
        return await _context.Orders
            .Include(o => o.Events)
            .FirstOrDefaultAsync(o => o.Id == id, token);
    }

    public IAsyncEnumerable<Guid> GetOrderIdsWithPendingEvents(
        DateTime createdBeforeTimestampUtc, CancellationToken token = default)
    {
        return _context.Orders
            .Where(o => o.Events.Any(e => e.Status == EventStatus.Pending && 
                                          e.CreatedTimestampUtc < createdBeforeTimestampUtc))
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .AsAsyncEnumerable();
    }
}