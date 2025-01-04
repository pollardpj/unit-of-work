using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Shared.Observability;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .CreateLogger();

        Log.Logger = logger;

        builder.Host.UseSerilog();

        return builder;
    }
}
