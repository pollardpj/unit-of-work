using Domain.Entities;
using Shared;

namespace Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserWithOrders(int id);
}