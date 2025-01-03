using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository;

public class MyAppUnitOfWorkFactory : IMyAppUnitOfWorkFactory
{
    private DbContextOptions<MyAppContext> _options;

    public MyAppUnitOfWorkFactory(string connectionString, ILoggerFactory loggerFactory)
    {
        _options = new DbContextOptionsBuilder<MyAppContext>()
            .UseSqlServer(connectionString)
            .UseLoggerFactory(loggerFactory)
            .Options;
    }

    public IMyAppUnitOfWork Create()
    {
        var context = new MyAppContext(_options);

        return new MyAppUnitOfWork(
            context, 
            new MyAppRepositories
            {
                OrderRepository = new OrderRepository(context),
                OrderEventRepository = new OrderEventRepository(context)
            });
    }
}
