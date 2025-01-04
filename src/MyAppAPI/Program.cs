using MyAppAPI.Extensions;
using Serilog;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

using var _ = builder.ConfigureLogging();

builder.Services.AddMyAppServices();

try
{
    Log.Information("Starting web application");

    var app = builder.Build();

    app.AddMyAppMiddleware();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
