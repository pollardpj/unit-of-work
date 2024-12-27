using System.Text.Json;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
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

// Create/Update

var unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>();

var user = new User
{
    Reference = Guid.NewGuid(),
    Name = "Phil",
    Email = "phil@pollard.co.uk"
};

await unitOfWork.UserRepository.AddAsync(user);

// Create an Order tied to the user
var order = new Order
{
    Reference = Guid.NewGuid(),
    ProductName = "Product #1", 
    Price = 1M, 
    User = user
};

order.Events.Add(new OrderEvent
{
    Reference = Guid.NewGuid(),
    CreatedTimestampUtc = DateTime.UtcNow,
    Status = EventStatus.Pending,
    Type = OrderEventType.Created,
    Payload = JsonSerializer.Serialize(new
    {
        order.Reference,
        order.ProductName,
        User = new
        {
            order.User.Reference,
            order.User.Name,
            order.User.Email
        }
    })
});
await unitOfWork.OrderRepository.AddAsync(order);

// var existingUser = await unitOfWork.UserRepository.GetByIdAsync(1);
// existingUser.Name = "Billy";
// unitOfWork.UserRepository.Update(existingUser);

// Save both User and Order in a single transaction
await unitOfWork.FlushAsync();

// Get Some Data:

unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>();

var myUser = await unitOfWork.UserRepository.GetUserWithOrders(Guid.Parse("34198673-604e-4b71-abeb-e464e1300b50"));
var myUserDto = new UserDto
{
    Reference = myUser.Reference,
    Name = myUser.Name,
    Email = myUser.Email,
    Orders = myUser.Orders.Select(o => new OrderDto
    {
        Reference = o.Reference,
        ProductName = o.ProductName,
        Price = o.Price,
        Events = o.Events.Select(e => new OrderEventDto
        {
            Reference = e.Reference,
            Type = e.Type,
            CreatedTimestampUtc = e.CreatedTimestampUtc,
            Payload = e.Payload,
            Status = e.Status
        })
    })
};

Console.WriteLine(JsonSerializer.Serialize(myUserDto));

var ordersWithPendingEvents = unitOfWork.OrderRepository.GetOrdersWithPendingEvents();

await foreach (var orderWithPendingEvents in ordersWithPendingEvents)
{
    Console.WriteLine($"Order {orderWithPendingEvents.Reference} has pending events.");
}

