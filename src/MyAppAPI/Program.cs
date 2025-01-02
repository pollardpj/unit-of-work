using AutoMapper;
using Dapr.Actors;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using MyAppAPI.Models;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.CQRS;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
{
    return new MyAppUnitOfWorkFactory("Server=(localdb)\\MSSQLLocalDB;Database=myapp");
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IValidator<OrderRequest>, OrderRequestValidator>();
builder.Services.AddScoped<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>();

builder.Services.AddScoped<ICommandHandler<CreateOrder>, CreateOrderHandler>();
builder.Services.AddScoped<IOrderEventsService, OrderEventsService>();
builder.Services.AddScoped<IQueryHandler<GetOrders, GetOrdersResult>, GetOrdersHandler>();

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
        IValidator<OrderRequest> validator,
        ICommandHandler <CreateOrder> handler,
        IMapper mapper) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var command = mapper.Map<CreateOrder>(request);

    await handler.ExecuteAsync(command);

    return Results.Ok(new
    {
        OrderReference = request.Reference,
        OrderPrice = command.Price
    });
});

app.MapGet("/api/orders",
    async (
        [AsParameters] GetOrdersRequest request,
        IValidator<GetOrdersRequest> validator,
        IQueryHandler<GetOrders, GetOrdersResult> handler,
        IMapper mapper) =>
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return Results.Ok(
            await handler.ExecuteAsync(mapper.Map<GetOrders>(request)));
    });

app.MapActorsHandlers();

app.Run();
