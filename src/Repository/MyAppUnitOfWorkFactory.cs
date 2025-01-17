using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppUnitOfWorkFactory(
    IDbContextFactory<MyAppContext> _contextFactory) : IMyAppUnitOfWorkFactory
{
    public async ValueTask<IMyAppUnitOfWork> CreateAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();

        return new MyAppUnitOfWork(
            context, 
            new MyAppRepositories
            {
                OrderRepository = new OrderRepository(context),
                OrderEventRepository = new OrderEventRepository(context)
            });
    }
}
