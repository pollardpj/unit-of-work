using Microsoft.AspNetCore.Builder;
using Serilog;
using SerilogTracing;

namespace Shared.Observability;

public static class LoggingExtensions
{
    public static LoggingInfo ConfigureLogging(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .Filter.ByExcluding("RequestPath like '%/healthz%'")
           .CreateLogger();

        Log.Logger = logger;

        builder.Host.UseSerilog();

        return new LoggingInfo
        {
            Logger = logger,
            TraceHandle = new ActivityListenerConfiguration()
                .Instrument.AspNetCoreRequests()
                .Instrument.HttpClientRequests()
                .TraceToSharedLogger()
        };
    }
}

public class LoggingInfo : IDisposable
{
    private bool _disposed;

    public IDisposable TraceHandle { get; set; }
    public ILogger Logger { get; set; }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TraceHandle?.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}