using Asp.Versioning;
using Dapr.Actors;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using MyAppAPI.Models;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.CQRS;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MyAppAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
        {
            return new MyAppUnitOfWorkFactory(
                "Server=(localdb)\\MSSQLLocalDB;Database=myapp",
                sp.GetService<ILoggerFactory>());
        });

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services
            .AddScoped<IValidator<OrderRequest>, OrderRequestValidator>()
            .AddScoped<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>()
            .AddFluentValidationAutoValidation();

        services
            .AddScoped<ICommandHandler<CreateOrder>, CreateOrderHandler>()
            .AddScoped<IOrderEventsService, OrderEventsService>();

        services
            .AddScoped<IQueryHandler<GetOrders, GetOrdersResult>, GetOrdersHandler>();

        services.AddActors(options =>
        {
            options.Actors.RegisterActor<OrderActor>();

            options.ActorIdleTimeout = TimeSpan.FromSeconds(5);

            options.ReentrancyConfig = new ActorReentrancyConfig
            {
                Enabled = false
            };
        });

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
