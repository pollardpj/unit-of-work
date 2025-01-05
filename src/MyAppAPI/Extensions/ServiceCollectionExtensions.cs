using Asp.Versioning;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.HostedServices;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using MyAppAPI.Models;
using MyAppAPI.Models.Validators;
using Repository;
using Shared;
using Shared.CQRS;
using Shared.Observability;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Reflection;

namespace MyAppAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyAppServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
            {
                return new MyAppUnitOfWorkFactory(
                    "Server=(localdb)\\MSSQLLocalDB;Database=myapp",
                    sp.GetService<ILoggerFactory>());
            });

        services
            .AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly());

        services
            .AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>()
            .AddScoped<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>()
            .AddFluentValidationAutoValidation();

        services
            .AddScoped<ICommandHandler<CreateOrder, CreateOrderResult>, CreateOrderHandler>()
            .Decorate<ICommandHandler<CreateOrder, CreateOrderResult>, LoggingCommandHandler<CreateOrder, CreateOrderResult>>()
            .Decorate<ICommandHandler<CreateOrder, CreateOrderResult>, TracingCommandHandler<CreateOrder, CreateOrderResult>>();

        services
            .AddScoped<IQueryHandler<GetOrders, GetOrdersResult>, GetOrdersHandler>()
            .Decorate<IQueryHandler<GetOrders, GetOrdersResult>, LoggingQueryHandler<GetOrders, GetOrdersResult>>()
            .Decorate<IQueryHandler<GetOrders, GetOrdersResult>, TracingQueryHandler<GetOrders, GetOrdersResult>>();

        services
            .AddScoped<IOrderEventsService, OrderEventsService>();

        services
            .AddHostedService<OrderCheckingService>();

        services
            .AddActors(options =>
            {
                options.Actors.RegisterActor<OrderActor>(typeOptions: new()
                {
                    ActorIdleTimeout = TimeSpan.FromSeconds(10)
                });

                options.Actors.RegisterActor<OrderSupervisorActor>(typeOptions: new()
                {
                    ActorIdleTimeout = TimeSpan.FromMinutes(2)
                });
            });

        services
            .Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonHelpers.DefaultOptions.PropertyNamingPolicy;

                foreach (var converter in JsonHelpers.DefaultOptions.Converters)
                {
                    options.SerializerOptions.Converters.Add(converter);
                }
            });

        return services;
    }
}
