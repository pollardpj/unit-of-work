using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

var services = new ServiceCollection();

services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

var serviceProvider = services.BuildServiceProvider();

var context = serviceProvider.GetService<MyAppContext>();
context.Database.EnsureCreated();
