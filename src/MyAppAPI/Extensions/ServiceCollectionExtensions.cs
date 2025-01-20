using System.Reflection;
using Asp.Versioning;
using Domain.Actors;
using Domain.Commands.Handlers;
using Domain.HostedServices;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.Caching;
using Shared.CQRS;
using Shared.Json;
using Shared.Observability;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace MyAppAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyAppServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddCore(config)

            // Application:

            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidation()
            .AddUnitOfWork(config)
            .AddCqrs()
            .AddScoped<IOrderEventsService, OrderEventsService>()
            .AddHostedService<OrderCheckingService>()
            .AddDaprServices();

        return services;
    }

    private static IServiceCollection AddCore(this IServiceCollection services, IConfiguration config)
    {
#pragma warning disable EXTEXP0018
        services
            .AddProblemDetails()
            .AddIdempotency(config)
            .AddVersioning()
            .ConfigureJson()
            .AddHybridCache(options =>
            {
                // Default timeouts
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromMinutes(30)
                };
            });
#pragma warning restore EXTEXP0018

        return services;
    }

    private static IServiceCollection AddIdempotency(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddIdempotentMinimalAPI(new IdempotencyOptions
            {
                HeaderKeyName = "Idempotency-Key",
                ExpiresInMilliseconds = 1000 * 60 * 10,
                DistributedCacheKeysPrefix = "Idempotency"
            })
            .AddIdempotentAPIUsingDistributedCache()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetValue<string>("redis.connectionstring");
                options.InstanceName = "MyApp";
            });

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
                options.SerializerOptions.PropertyNameCaseInsensitive = JsonHelpers.DefaultOptions.PropertyNameCaseInsensitive;

                foreach (var converter in JsonHelpers.DefaultOptions.Converters)
                {
                    options.SerializerOptions.Converters.Add(converter);
                }
            });

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContextFactory<MyAppContext>(options =>
        {
            options.UseSqlServer(config.GetValue<string>("database.connectionstring"));
        });

        services.AddSingleton<Func<MyAppContext, MyAppRepositories>>(_ =>
        {
            return ctx => new MyAppRepositories
            {
                OrderEventRepository = new OrderEventRepository(ctx),
                OrderRepository = new OrderRepository(ctx)
            };
        });

        services
            .AddSingleton<IMyAppUnitOfWorkFactory, MyAppUnitOfWorkFactory>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>()
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
            .Decorate(typeof(IQueryHandler<GetOrder, GetOrderResult>), typeof(CachingQueryHandler<GetOrder, GetOrderResult>)) 
            .Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>))
            .Decorate(typeof(IQueryHandler<,>), typeof(TracingQueryHandler<,>));

        return services;
    }
}
