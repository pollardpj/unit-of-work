using Asp.Versioning;
using AutoMapper;
using Dapr.Actors;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.Exceptions;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using MyAppAPI.Models;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.CQRS;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
{
    return new MyAppUnitOfWorkFactory(
        "Server=(localdb)\\MSSQLLocalDB;Database=myapp", 
        sp.GetService<ILoggerFactory>());
});

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.Configure<RouteHandlerOptions>(options =>
{
    options.ThrowOnBadRequest = true;
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services
    .AddScoped<IValidator<OrderRequest>, OrderRequestValidator>()
    .AddScoped<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>()
    .AddFluentValidationAutoValidation();

builder.Services
    .AddScoped<ICommandHandler<CreateOrder>, CreateOrderHandler>()
    .AddScoped<IOrderEventsService, OrderEventsService>();

builder.Services
    .AddScoped<IQueryHandler<GetOrders, GetOrdersResult>, GetOrdersHandler>();

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

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

app.MapPost("/api/{version:apiVersion}/order",
    async (
        OrderRequest request,
        ICommandHandler<CreateOrder> handler,
        IMapper mapper) =>
    {
        var command = mapper.Map<CreateOrder>(request);

        await handler.ExecuteAsync(command);

        return Results.Ok(new
        {
            OrderReference = request.Reference,
            OrderPrice = command.Price
        });

    })
    .AddFluentValidationAutoValidation()
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1);

app.MapGet("/api/{version:apiVersion}/orders",
    async (
        [AsParameters] GetOrdersRequest request,
        IQueryHandler<GetOrders, GetOrdersResult> handler,
        IMapper mapper) =>
    {
        try
        {
            return Results.Ok(
                await handler.ExecuteAsync(mapper.Map<GetOrders>(request)));
        }
        catch (PagedQueryException ex)
        {
            return Results.BadRequest(ex.Details);
        }

    })
    .AddFluentValidationAutoValidation()
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1);

app.MapActorsHandlers();

app.Run();
