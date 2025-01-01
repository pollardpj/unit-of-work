using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Repository;

public class OrderRepository(MyAppContext context) 
    : Repository<Order>(context), IOrderRepository
{
    public async ValueTask<Order> GetOrderWithEvents(Guid reference)
    {
        return await context.Set<Order>()
            .Include(u => u.Events)
            .FirstOrDefaultAsync(u => u.Reference == reference);
    }

    public IAsyncEnumerable<Guid> GetOrderReferencesWithPendingEvents()
    {
        return context.Set<Order>()
            .Where(o => o.Events.Any(e => e.Status == EventStatus.Pending))
            .Select(o => o.Reference)
            .AsAsyncEnumerable();
    }
}