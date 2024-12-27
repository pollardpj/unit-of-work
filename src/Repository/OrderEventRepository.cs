using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Repository;

public class OrderEventRepository(MyAppContext context) 
    : Repository<OrderEvent>(context), IOrderEventRepository
{
    public async Task<IEnumerable<OrderEvent>> GetPendingEvents(Guid orderReference)
    {
        return await context.Set<Order>()
            .Where(o => o.Reference == orderReference)
            .SelectMany(o => o.Events
                .Where(e => e.Status == EventStatus.Pending))
            .OrderBy(e => e.CreatedTimestampUtc)
            .ToListAsync();
    }
}