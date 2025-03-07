﻿using System.Reflection;
using Domain.Actors;
using Domain.Commands;
using Domain.Commands.Handlers;
using Domain.HostedServices;
using Domain.Queries;
using Domain.Queries.Handlers;
using Domain.Services;
using Domain.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyAppAPI.Models.Validators;
using Repository;
using Shared.CQRS;
using Shared.CQRS.Decorators;
using Shared.Extensions;
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
            .Decorate(typeof(ICommandHandler<UpdateOrder, UpdateOrderResult>), 
                typeof(CacheInvalidatingCommandHandler<UpdateOrder, UpdateOrderResult>))
            .Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>))
            .Decorate(typeof(ICommandHandler<,>), typeof(TracingCommandHandler<,>));

        services.Scan(scan => scan
            .FromAssemblyOf<GetOrdersHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());

        services
            .Decorate(typeof(IQueryHandler<GetOrder, GetOrderResult>), 
                typeof(CachingQueryHandler<GetOrder, GetOrderResult>)) 
            .Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>))
            .Decorate(typeof(IQueryHandler<,>), typeof(TracingQueryHandler<,>));

        return services;
    }
}
