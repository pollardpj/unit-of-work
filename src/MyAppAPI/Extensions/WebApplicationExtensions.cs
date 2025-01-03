﻿using Asp.Versioning;
using AutoMapper;
using Dapr;
using Domain.Commands;
using Domain.Events;
using Domain.Queries;
using MyAppAPI.Models;
using Shared;
using Shared.CQRS;
using Shared.Exceptions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Text.Json;

namespace MyAppAPI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication AddMyAppMiddleware(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .Build();

        // Dapr will send serialized event object vs. being raw CloudEvent
        app.UseCloudEvents();

        // needed for Dapr pub/sub routing
        app.MapSubscribeHandler();

        app.MapPost("/api/{version:apiVersion}/order",
            async (
                CreateOrderRequest request,
                ICommandHandler<CreateOrder> handler,
                IMapper mapper,
                CancellationToken token = default) =>
            {
                var command = mapper.Map<CreateOrder>(request);

                await handler.ExecuteAsync(command, token);

                return Results.Ok(new
                {
                    OrderReference = command.Reference,
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
                IMapper mapper,
                CancellationToken token = default) =>
            {
                try
                {
                    return Results.Ok(
                        await handler.ExecuteAsync(mapper.Map<GetOrders>(request), token));
                }
                catch (PagedQueryException ex)
                {
                    return Results.BadRequest(ex.Details);
                }

            })
            .AddFluentValidationAutoValidation()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        // Dapr subscription in [Topic] routes orders topic to this route
        app.MapPost("/orderevent", [Topic("order-pubsub", "order-event")] 
            (
                ILogger<Program> logger,
                OrderEventPayload order,
                CancellationToken token = default) =>
            {
                logger.LogInformation("Order Created Message Received: {Order}", 
                    JsonSerializer.Serialize(order, JsonHelpers.DefaultOptions));
            });

        app.MapActorsHandlers();

        return app;
    }
}
