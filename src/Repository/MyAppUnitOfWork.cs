using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace Repository;

public class MyAppUnitOfWork(MyAppContext context, IServiceProvider serviceProvider) 
    : UnitOfWork(context, serviceProvider), IMyAppUnitOfWork
{
    public IUserRepository UserRepository => ServiceProvider.GetService<IUserRepository>();
    public IOrderRepository OrderRepository => ServiceProvider.GetService<IOrderRepository>();
}