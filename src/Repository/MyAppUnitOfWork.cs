using Domain.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace Repository;

public class MyAppUnitOfWork(MyAppContext context, IServiceProvider serviceProvider) 
    : UnitOfWork(context, serviceProvider), IMyAppUnitOfWork
{
    public IOrderRepository OrderRepository => ServiceProvider.GetService<IOrderRepository>();
    public IOrderEventRepository OrderEventRepository => ServiceProvider.GetService<IOrderEventRepository>();
}