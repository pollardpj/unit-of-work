using Dapr.Actors;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using MyAppAPI.Models;
using Repository;
using Shared.CQRS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
{
    return new MyAppUnitOfWorkFactory("Server=(localdb)\\MSSQLLocalDB;Database=myapp");
});

builder.Services.AddScoped<ICommandHandler<CreateOrder>, CreateOrderHandler>();
builder.Services.AddScoped<IOrderEventsService, OrderEventsService>();

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
    async (OrderRequest request, ICommandHandler<CreateOrder> createOrderHandler) =>
{
    var command = new CreateOrder
    {
        Reference = request.Reference,
        ProductName = request.ProductName
    };

    await createOrderHandler.ExecuteAsync(command);

    return Results.Ok(new
    {
        OrderReference = request.Reference,
        OrderPrice = command.Price
    });
});

app.MapActorsHandlers();

app.Run();
