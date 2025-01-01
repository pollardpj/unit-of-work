using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OrderRepository(MyAppContext context) 
    : Shared.Repository<Order>(context), IOrderRepository
{
    public async Task<Order> GetOrderWithEvents(Guid reference)
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