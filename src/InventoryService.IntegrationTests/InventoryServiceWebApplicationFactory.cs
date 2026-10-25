using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace InventoryService.IntegrationTests;

/// <summary>
/// Intgeration test web application factory.
/// </summary>
public sealed class InventoryServiceWebApplicationFactory : WebApplicationFactory<Program>
{
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
    }
}