using MyAppAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyAppServices();

var app = builder.Build();

app.AddMyAppMiddleware();

app.Run();
