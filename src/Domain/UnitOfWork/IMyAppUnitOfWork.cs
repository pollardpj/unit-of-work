using Shared;

namespace Domain.UnitOfWork;

public interface IMyAppUnitOfWork : IUnitOfWork
{
    IOrderRepository OrderRepository { get; }
    IOrderEventRepository OrderEventRepository { get; }
}