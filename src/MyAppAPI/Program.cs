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

builder.Services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=myapp"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderEventRepository, OrderEventRepository>();
builder.Services.AddScoped<IMyAppUnitOfWork, MyAppUnitOfWork>();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<OrderActor>();

    options.ReentrancyConfig = new ActorReentrancyConfig
    {
        Enabled = false
    };
});

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

app.MapPost("/api/order", 
    async (
        OrderRequest request, 
        IMyAppUnitOfWork unitOfWork,
        ILogger<Program> logger) =>
{
    
    #region Update Database

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

    #endregion

    #region Publish Event

    try
    {
        var actorId = new ActorId($"order-{request.Reference}");
        var proxy = ActorProxy.Create<IOrderActor>(actorId, nameof(OrderActor));
        await proxy.PublishEvents(request.Reference);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, ex.Message);
    }

    #endregion

    return Results.Ok(new
    {
        OrderReference = order.Reference
    });
});

app.MapActorsHandlers();

app.Run();
