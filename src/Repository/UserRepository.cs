using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class UserRepository(MyAppContext context) 
    : Shared.Repository<User>(context), IUserRepository
{
    public async Task<User> GetUserWithOrders(Guid reference)
    {
        return await context.Set<User>()
            .Include(u => u.Orders)
            .ThenInclude(o => o.Events)
            .FirstOrDefaultAsync(u => u.Reference == reference);
    }
}