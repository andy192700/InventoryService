using InventoryService.Extensions;
using InventoryService.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

var app = builder.Build();

app
    .RegisterMiddleware()
    .UseEndpoints(endpoints => endpoints.RegisterEndpoints());

app.Run();

/// <summary>
/// Partial <see cref="Program"/> class, in place for assembly scanning reasons (MediatR/Integration tests).
/// </summary>
public partial class Program { }