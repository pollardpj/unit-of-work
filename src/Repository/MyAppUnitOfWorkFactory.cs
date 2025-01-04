using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository;

public class MyAppUnitOfWorkFactory(
    string _connectionString, 
    ILoggerFactory _loggerFactory) : IMyAppUnitOfWorkFactory
{
    private readonly DbContextOptions<MyAppContext> _options = 
        new DbContextOptionsBuilder<MyAppContext>()
            .UseSqlServer(_connectionString)
            .UseLoggerFactory(_loggerFactory)
            .Options;

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
