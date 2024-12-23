using System.Text.Json;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

var services = new ServiceCollection();

services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IMyAppUnitOfWork, MyAppUnitOfWork>();

var serviceProvider = services.BuildServiceProvider();

var context = serviceProvider.GetService<MyAppContext>();
context.Database.EnsureCreated();

using (var unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>())
{
    var user = new User { Name = "Phil", Email = "phil@pollard.co.uk" };
    await unitOfWork.UserRepository.AddAsync(user);

    // Create an Order tied to the user
    var order = new Order { ProductName = "Product #1", Price = 1M, User = user };
    await unitOfWork.OrderRepository.AddAsync(order);

    var existingUser = await unitOfWork.UserRepository.GetByIdAsync(4);
    existingUser.Name = "Billy";
    unitOfWork.UserRepository.Update(existingUser);

    // Save both User and Order in a single transaction
    await unitOfWork.FlushAsync();
}

using (var unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>())
{
    var myUser = await unitOfWork.UserRepository.GetUserWithOrders(4);
    var myUserDto = new UserDto
    {
        Id = myUser.Id,
        Name = myUser.Name,
        Email = myUser.Email,
        Orders = myUser.Orders.Select(o => new OrderDto
        {
            ProductName = o.ProductName,
            Price = o.Price
        })
    };
    
    Console.WriteLine(JsonSerializer.Serialize(myUserDto));
}
