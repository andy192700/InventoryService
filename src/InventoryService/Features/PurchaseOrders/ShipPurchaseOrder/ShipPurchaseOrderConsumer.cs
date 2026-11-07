using Messaging;

namespace InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;

public record ShipPurchaseOrderRequest
(
    int PurchaseOrderId
);

public class ShipPurchaseOrderDescriptor : IQueueDescriptor
{
    /// <inheritdoc/>
    public string TopicName => "ShipPurchaseOrder";

    /// <inheritdoc/>
    public string QueueName => "ShipPurchaseOrder";
}

public class ShipPurchaseOrderConsumer : SQSConsumer<ShipPurchaseOrderRequest, ShipPurchaseOrderDescriptor>
{
    public ShipPurchaseOrderConsumer(
        ISQSSource<ShipPurchaseOrderDescriptor> source,
        ILogger<ShipPurchaseOrderConsumer> logger)
        : base(source, logger)
    {

    }

    /// <inheritdoc/>
    protected override Task HandleAsync(ShipPurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has completed order {payload.PurchaseOrderId}, marking as shipped.");

        return Task.CompletedTask;
    }
}
