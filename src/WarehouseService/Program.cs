using Messaging;
using WarehouseService;

var host = new HostBuilder();

host.ConfigureLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    config.AddEnvironmentVariables();
});

host.ConfigureServices((ctx, services) =>
{
    services.AddAWS(ctx.Configuration);
    services.AddConsumer<PurchaseOrderConsumer, PurchaseOrderConsumerDescriptor>();
    services.AddProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic>();
});

await host.RunConsoleAsync();