using Asp.Versioning;
using AutoMapper;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Queries;
using MyAppAPI.Models;
using Shared.CQRS;
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

        return app;
    }
}
