using Microsoft.AspNetCore.Builder;
using Serilog;
using SerilogTracing;

namespace Shared.Observability;

public static class ObservabilityExtensions
{
    public static BootstrapLogger ConfigureLogging(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .CreateLogger();

        Log.Logger = logger;

        builder.Host.UseSerilog();

        return new BootstrapLogger
        {
            Logger = logger,
            TraceHandle = new ActivityListenerConfiguration()
                .Instrument.AspNetCoreRequests()
                .Instrument.HttpClientRequests()
                .TraceToSharedLogger()
        };
    }
}

public class BootstrapLogger : IDisposable
{
    private bool _disposed;

    public IDisposable TraceHandle { get; set; }
    public ILogger Logger { get; set; }

    public void CloseAndFlush() => Log.CloseAndFlush();

    public void Information(string message) => Log.Information(message);

    public void Fatal(Exception ex, string message) => Log.Fatal(ex, message);

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