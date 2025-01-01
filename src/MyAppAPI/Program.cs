using Dapr.Actors;
using Dapr.Actors.Client;
using Domain.Actors;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using MyAppAPI.Models;
using Repository;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
{
    return new MyAppUnitOfWorkFactory("Server=(localdb)\\MSSQLLocalDB;Database=myapp");
});

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<OrderActor>();

    options.ActorIdleTimeout = TimeSpan.FromSeconds(5);

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
        IMyAppUnitOfWorkFactory unitOfWorkFactory,
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
            Type = OrderEventType.Created,
            ProductName = order.ProductName,
            Price = order.Price
        })
    });

    using (var unitOfWork = unitOfWorkFactory.Create())
    {
        unitOfWork.OrderRepository.Add(order);
        await unitOfWork.FlushAsync();
    }

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
