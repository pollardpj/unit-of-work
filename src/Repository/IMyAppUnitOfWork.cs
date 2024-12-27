using Shared;

namespace Repository;

public interface IMyAppUnitOfWork : IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IOrderRepository OrderRepository { get; }
}