using Dapr.Actors;
using Dapr.Actors.Client;
using Domain.Actors;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MyAppAPI.Models;
using Repository;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddActors(options =>
//{
//    options.Actors.RegisterActor<OrderActor>();

//    options.ReentrancyConfig = new ActorReentrancyConfig
//    {
//        Enabled = false
//    };
//});

builder.Services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMyAppUnitOfWork, MyAppUnitOfWork>();

var app = builder.Build();

app.MapPost("/order", 
    async (
        OrderRequest request, 
        IMyAppUnitOfWork unitOfWork,
        ILogger<Program> logger) =>
{
    var order = new Order
    {
        Reference = request.Reference,
        ProductName = request.ProductName,
        Price = 1.99M
    };

    order.Events.Add(new OrderEvent
    {
        Reference = order.Reference,
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

    await unitOfWork.FlushAsync();

    //try
    //{
    //    var actorId = new ActorId($"order-{request.Reference}");
    //    var proxy = ActorProxy.Create<IOrderActor>(actorId, nameof(OrderActor));
    //    await proxy.PublishEvents(request.Reference);
    //}
    //catch (Exception e)
    //{
    //    logger.LogError(e, e.Message);
    //}

    return Results.Ok(new
    {
        OrderId = order.Id
    });
});

app.Run();
