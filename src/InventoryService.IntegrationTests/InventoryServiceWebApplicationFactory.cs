using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.IntegrationTests;

/// <summary>
/// Intgeration test web application factory.
/// </summary>
public sealed class InventoryServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Override test services to ensure test specific behaviour can be achieved
    /// </summary>
    public Action<IServiceCollection>? OverrideTestServices { get; set; }

    private readonly IntegrationTestFixture _sqlFixture;

    public InventoryServiceWebApplicationFactory(IntegrationTestFixture sqlFixture)
    {
        _sqlFixture = sqlFixture;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Inventory"] = _sqlFixture.ConnectionString
                };

                config.AddInMemoryCollection(settings);
            });

        builder.ConfigureTestServices(x =>
        {
            OverrideTestServices?.Invoke(x);
        });
    }
}