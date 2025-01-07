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
using Shared.Exceptions;
using Shared.Filters;
using Shared.Json;
using Shared.Validation;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace MyAppAPI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication AddMyAppMiddleware(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .Build();

        app.MapDaprMiddleware();

        app.MapPost("/api/{version:apiVersion}/order",
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

            })
            .AddEndpointFilter<BadRequestFilter>()
            .AddEndpointFilter<IdempotentAPIEndpointFilter>()
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        app.MapPut("/api/{version:apiVersion}/orders/{orderId:guid}",
            async (
                Guid orderId,
                UpdateOrderRequest request,
                ICommandHandler<UpdateOrder, UpdateOrderResult> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                var command = mapper.Map<UpdateOrder>(request);
                command.Id = orderId;

                try
                {
                    var result = await handler.ExecuteAsync(command, token);

                    return Results.Ok(new
                    {
                        OrderId = result.Id,
                        OrderPrice = result.Price
                    });
                }
                catch (ConflictException)
                {
                    return Results.Conflict();
                }
            })
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        app.MapGet("/api/{version:apiVersion}/orders",
            async (
                [AsParameters] GetOrdersRequest request,
                IQueryHandler<GetOrders, GetOrdersResult> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                try
                {
                    return Results.Ok(
                        await handler.ExecuteAsync(mapper.Map<GetOrders>(request), token));
                }
                catch (BadRequestException ex)
                {
                    return Results.Problem(ex.Message.GetProblemDetails());
                }

            })
            .AddEndpointFilter<BadRequestFilter>()
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

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
