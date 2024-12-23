using Shared;

namespace Repository;

public interface IMyAppUnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IOrderRepository OrderRepository { get; }
}