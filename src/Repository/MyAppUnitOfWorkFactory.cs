using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppUnitOfWorkFactory : IMyAppUnitOfWorkFactory
{
    private DbContextOptions<MyAppContext> _options;

    public MyAppUnitOfWorkFactory(string connectionString)
    {
        _options = new DbContextOptionsBuilder<MyAppContext>()
            .UseSqlServer(connectionString)
            .Options;
    }

    public IMyAppUnitOfWork Create()
    {
        var context = new MyAppContext(_options);

        return new MyAppUnitOfWork(
            context, 
            new OrderRepository(context), 
            new OrderEventRepository(context));
    }
}
