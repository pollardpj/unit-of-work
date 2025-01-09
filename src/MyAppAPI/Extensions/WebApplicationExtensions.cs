using System.Text.Json;
using Asp.Versioning;
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
        app
            .UseExceptionHandler()
            .UseStatusCodePages();

        app.MapDaprMiddleware();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        var groupv1 = app.MapGroup("/api/{version:apiVersion}")
            .AddEndpointFilter<BadRequestFilter>()
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        var groupv1WithIdempotency = app.MapGroup("/api/{version:apiVersion}")
            .AddEndpointFilter<BadRequestFilter>()
            .AddEndpointFilter<IdempotentAPIEndpointFilter>()
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        groupv1WithIdempotency.MapPost("/order",
            async (
                CreateOrderRequest request,
                ICommandHandler<CreateOrder, CreateOrderResult> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                var command = mapper.Map<CreateOrder>(request);

                var result = await handler.ExecuteAsync(command, token);

                return Results.Ok(new
                {
                    OrderId = result.Id,
                    OrderPrice = result.Price
                });

            });

        groupv1.MapPut("/orders/{orderId:guid}",
            async (
                Guid orderId,
                UpdateOrderRequest request,
                ICommandHandler<UpdateOrder, UpdateOrderResult> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                var command = mapper.Map<UpdateOrder>(request);
                command.Id = orderId;

                var result = await handler.ExecuteAsync(command, token);

                return Results.Ok(new
                {
                    OrderId = result.Id,
                    OrderPrice = result.Price
                });

            });

        groupv1.MapGet("/orders",
            async (
                [AsParameters] GetOrdersRequest request,
                IQueryHandler<GetOrders, GetOrdersResult> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                return Results.Ok(
                    await handler.ExecuteAsync(mapper.Map<GetOrders>(request), token));
            });

        return app;
    }

    private static WebApplication MapDaprMiddleware(this WebApplication app)
    {
        // Dapr will send serialized event object vs. being raw CloudEvent
        app.UseCloudEvents();

        // needed for Dapr pub/sub routing
        app.MapSubscribeHandler();

        app.MapActorsHandlers();

        // Dapr subscription in [Topic] routes orders topic to this route
        app.MapPost("/orderevent", [Topic("order-pubsub", "order-event")]
            (
                OrderEventPayload @event,
                ILogger<Program> logger,
                CancellationToken token = default) =>
            {
                logger.LogInformation("Order Event Received for {OrderId}: {Event}",
                    @event.OrderId, JsonSerializer.Serialize(@event, JsonHelpers.DefaultOptions));
            });

        return app;
    }
}
