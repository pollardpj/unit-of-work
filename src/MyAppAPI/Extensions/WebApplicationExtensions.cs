using System.Text.Json;
using Asp.Versioning;
using Asp.Versioning.Builder;
using AutoMapper;
using Dapr;
using Domain.Commands;
using Domain.Events;
using Domain.Queries;
using IdempotentAPI.MinimalAPI;
using MyAppAPI.Models;
using Shared.CQRS;
using Shared.Filters;
using Shared.Json;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace MyAppAPI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication AddMyAppMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler()
           .UseStatusCodePages();

        app.MapDaprMiddleware();

        var versionSet = CreateApiVersionSet(app);
        var (groupV1, groupV1WithIdempotency) = CreateApiGroups(app, versionSet);

        MapOrderEndpoints(groupV1, groupV1WithIdempotency);

        return app;
    }

    private static ApiVersionSet CreateApiVersionSet(WebApplication app) =>
        app.NewApiVersionSet()
           .HasApiVersion(new ApiVersion(1))
           .Build();

    private static (IEndpointRouteBuilder standard, IEndpointRouteBuilder idempotent) CreateApiGroups(
        WebApplication app, 
        ApiVersionSet versionSet)
    {
        var baseGroup = app.MapGroup("/api/v{version:apiVersion}")
            .AddEndpointFilter<BadRequestFilter>()
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        var standardGroup = baseGroup;
        var idempotentGroup = baseGroup.AddEndpointFilter<IdempotentAPIEndpointFilter>();

        return (standardGroup, idempotentGroup);
    }

    private static void MapOrderEndpoints(
        IEndpointRouteBuilder standardGroup, 
        IEndpointRouteBuilder idempotentGroup)
    {
        idempotentGroup.MapPost("/orders", HandleCreateOrder);
        standardGroup.MapPut("/orders/{orderId:guid}", HandleUpdateOrder);
        standardGroup.MapGet("/orders", HandleGetOrders);
    }

    private static async Task<IResult> HandleCreateOrder(
        CreateOrderRequest request,
        ICommandHandler<CreateOrder, CreateOrderResult> handler,
        IMapper mapper,
        CancellationToken token = default)
    {
        var result = await handler.ExecuteAsync(
            mapper.Map<CreateOrder>(request), 
            token);

        return Results.Ok(new { OrderId = result.Id, OrderPrice = result.Price });
    }

    private static async Task<IResult> HandleUpdateOrder(
        Guid orderId,
        UpdateOrderRequest request,
        ICommandHandler<UpdateOrder, UpdateOrderResult> handler,
        IMapper mapper,
        CancellationToken token = default)
    {
        var command = mapper.Map<UpdateOrder>(request);
        command.Id = orderId;

        var result = await handler.ExecuteAsync(command, token);

        return Results.Ok(new { OrderId = result.Id, OrderPrice = result.Price });
    }

    private static async Task<IResult> HandleGetOrders(
        [AsParameters] GetOrdersRequest request,
        IQueryHandler<GetOrders, GetOrdersResult> handler,
        IMapper mapper,
        CancellationToken token = default)
    {
        return Results.Ok(
            await handler.ExecuteAsync(mapper.Map<GetOrders>(request), token));
    }

    private static WebApplication MapDaprMiddleware(this WebApplication app)
    {
        app.UseCloudEvents();
        app.MapSubscribeHandler();
        app.MapActorsHandlers();

        app.MapPost("/orderevent/v1", 
            [Topic("order-pubsub", "order-event", "event.type.endsWith(\"v1\")", 1)]
            async (OrderEventPayload @event, ILogger<Program> logger, CancellationToken token) =>
            {
                logger.LogInformation("Version 1 Order Event Received for {OrderId}: {Event}",
                    @event.OrderId, 
                    JsonSerializer.Serialize(@event, JsonHelpers.DefaultOptions));
            });

        return app;
    }
}
