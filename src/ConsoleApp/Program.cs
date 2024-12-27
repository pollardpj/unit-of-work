using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

var services = new ServiceCollection();

services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IOrderEventRepository, OrderEventRepository>();
services.AddScoped<IMyAppUnitOfWork, MyAppUnitOfWork>();

var serviceProvider = services.BuildServiceProvider();

var context = serviceProvider.GetService<MyAppContext>();
context.Database.EnsureCreated();

// Create/Update

var unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>();

// Create an Order
var order = new Order
{
    Reference = Guid.NewGuid(),
    ProductName = "Product #1", 
    Price = 1M
};

order.Events.Add(new OrderEvent
{
    Reference = Guid.NewGuid(),
    CreatedTimestampUtc = DateTime.UtcNow,
    Status = EventStatus.Pending,
    Type = OrderEventType.Created,
    Payload = JsonSerializer.Serialize(new OrderEventPayload
    {
        Reference = order.Reference,
        ProductName = order.ProductName,
        Price = order.Price
    })
});
unitOfWork.OrderRepository.Add(order);

// var existingUser = await unitOfWork.UserRepository.GetByIdAsync(1);
// existingUser.Name = "Billy";
// unitOfWork.UserRepository.Update(existingUser);

// Save both User and Order in a single transaction
await unitOfWork.FlushAsync();

// Get Some Data:

unitOfWork = serviceProvider.GetService<IMyAppUnitOfWork>();

var orderReferences = unitOfWork.OrderRepository.GetOrderReferencesWithPendingEvents();

await foreach (var reference in orderReferences)
{
    Console.WriteLine($"Order {reference} has pending events.");
}

