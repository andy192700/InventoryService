using InventoryService;
using InventoryService.Features;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints => app.RegisterEndpoints());

app.Run();

/// <summary>
/// Partial <see cref="Program"/> class, in place for assembly scanning reasons (MediatR/Integration tests).
/// </summary>
public partial class Program { }