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

public partial class Program { }