using InventoryService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

var app = builder.Build();

app.RegisterMiddleware();

app.Run();

/// <summary>
/// Partial <see cref="Program"/> class, in place for Integration tests.
/// </summary>
public partial class Program { }