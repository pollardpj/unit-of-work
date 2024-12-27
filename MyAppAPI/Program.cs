using System.Text.Json;
using Dapr.Actors;
using Dapr.Actors.Client;
using Domain;
using Domain.Actors;
using Domain.Entities;
using Domain.Enums;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MyAppAPI.Models;
using Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<OrderActor>();

    options.ReentrancyConfig = new ActorReentrancyConfig
    {
        Enabled = false
    };
});

builder.Services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMyAppUnitOfWork, MyAppUnitOfWork>();

var app = builder.Build();

app.MapPost("/order", async (OrderRequest request, IMyAppUnitOfWork unitOfWork) =>
{
    var order = new Order
    {
        Reference = request.Reference,
        ProductName = request.ProductName,
        Price = 1.99M
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
            order.ProductName
        })
    });
    
    unitOfWork.OrderRepository.Add(order);

    await unitOfWork.FlushAsync();

    var actorId = new ActorId($"order-{request.Reference}");
    var proxy = ActorProxy.Create<IOrderActor>(actorId, nameof(OrderActor));
    await proxy.PublishEvents(request.Reference);

    return Results.Ok();
});

app.Run();
