using Domain.UnitOfWork;
using Shared.Repository;

namespace Repository;

public class MyAppUnitOfWork(
    MyAppContext _context, 
    MyAppRepositories _repositories) : UnitOfWork(_context), IMyAppUnitOfWork
{
    public IOrderRepository OrderRepository => _repositories.OrderRepository;
    public IOrderEventRepository OrderEventRepository => _repositories.OrderEventRepository;
}

public class MyAppRepositories
{
    public required IOrderRepository OrderRepository { get; init; }
    public required IOrderEventRepository OrderEventRepository { get; init; }
}