using Asp.Versioning;
using Domain.Actors;
using Domain.Commands.Handlers;
using Domain.HostedServices;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using MyAppAPI.Models;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.CQRS;
using Shared.Json;
using Shared.Observability;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Reflection;

namespace MyAppAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyAppServices(this IServiceCollection services)
    {
        // Core:

        services
            .AddVersioning()
            .ConfigureJson();

        // Application:

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidation()
            .AddUnitOfWork()
            .AddCqrs()
            .AddScoped<IOrderEventsService, OrderEventsService>()
            .AddHostedService<OrderCheckingService>()
            .AddDaprServices();

        return services;
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

        return services;
    }

    private static IServiceCollection ConfigureJson(this IServiceCollection services)
    {
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

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services
            .AddSingleton<IMyAppUnitOfWorkFactory>(sp =>
            {
                return new MyAppUnitOfWorkFactory(
                    "Server=(localdb)\\MSSQLLocalDB;Database=myapp",
                    sp.GetService<ILoggerFactory>());
            });

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>()
            .AddScoped<IValidator<GetOrdersRequest>, GetOrdersRequestValidator>()
            .AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection AddDaprServices(this IServiceCollection services)
    {
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

        return services;
    }

    private static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<CreateOrderHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());

        services
            .Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>))
            .Decorate(typeof(ICommandHandler<,>), typeof(TracingCommandHandler<,>));

        services.Scan(scan => scan
            .FromAssemblyOf<GetOrdersHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());

        services
            .Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>))
            .Decorate(typeof(IQueryHandler<,>), typeof(TracingQueryHandler<,>));

        return services;
    }
}
