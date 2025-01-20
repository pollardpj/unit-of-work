using Asp.Versioning;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration config)
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
}

