using Amazon.SimpleNotificationService;
using Amazon.SQS;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;
using InventoryService.IntegrationTests.Features.PurchaseOrders;
using Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
            x.RemoveAll<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>>();
            x.AddScoped<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>, CreatePurchaseOrderProducerStub>();

            x.RemoveAll<IHostedService>();
            x.RemoveAll<ISQSReader<ShipPurchaseOrderDescriptor>>();
            x.RemoveAll<SQSSource<ShipPurchaseOrderDescriptor>>();
            x.RemoveAll<IQueueFactory<ShipPurchaseOrderDescriptor>>();
            x.RemoveAll<IAmazonSQS>();
            x.RemoveAll<IAmazonSimpleNotificationService>();

            OverrideTestServices?.Invoke(x);
        });
    }
}