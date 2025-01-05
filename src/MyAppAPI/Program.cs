using MyAppAPI.Extensions;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

using var bootstrapLogger = builder.ConfigureLogging();

builder.Services.AddMyAppServices();

try
{
    bootstrapLogger.Information("Starting web application");

    var app = builder.Build();

    app.AddMyAppMiddleware();

    app.Run();
}
catch (Exception ex)
{
    bootstrapLogger.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    bootstrapLogger.CloseAndFlush();
}
