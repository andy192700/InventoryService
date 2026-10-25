using InventoryService;
using InventoryService.Features;
using InventoryService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.RegisterServices();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.RegisterEndpoints();

app.UseRouting();

app.Run();

/// <summary>
/// Partial <see cref="Program"/> class, in place for assembly scanning reasons (MediatR/Integration tests).
/// </summary>
public partial class Program { }