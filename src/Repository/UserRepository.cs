using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class UserRepository(MyAppContext context) 
    : Shared.Repository<User>(context), IUserRepository
{
    public async Task<User> GetUserWithOrders(int id)
    {
        return await context.Set<User>()
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}