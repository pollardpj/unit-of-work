using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppUnitOfWorkFactory(
    IDbContextFactory<MyAppContext> _contextFactory,
    Func<MyAppContext, MyAppRepositories> _repositoryFactory) : IMyAppUnitOfWorkFactory
{
    public async ValueTask<IMyAppUnitOfWork> CreateAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();

        return new MyAppUnitOfWork(context,_repositoryFactory(context));
    }
}
