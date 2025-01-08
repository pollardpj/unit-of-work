using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

Console.WriteLine("Migrations Application Initialised");

public class MyAppContextFactory : IDesignTimeDbContextFactory<MyAppContext>
{
    public MyAppContext CreateDbContext(string[] args)
    {
        var connectionString = args.Length > 0 ? args[0] : "Server=(localdb)\\MSSQLLocalDB;Database=myapp_migrations";

        var options = new DbContextOptionsBuilder<MyAppContext>()
            .UseSqlServer(connectionString, b => b.MigrationsAssembly("Migrations"))
            .Options;

        return new MyAppContext(options);
    }
}
