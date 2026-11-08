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
    private readonly IShipPurchaseOrderDBCommand _dbCommand;

    public ShipPurchaseOrderConsumer(
        ISQSSource<ShipPurchaseOrderDescriptor> source,
        IShipPurchaseOrderDBCommand dbCommand,
        ILogger<ShipPurchaseOrderConsumer> logger)
        : base(source, logger)
    {
        _dbCommand = dbCommand;
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(ShipPurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has completed order {payload.PurchaseOrderId}, marking as shipped.");

        var updateResult = await _dbCommand.MarkAsShippedAsync(payload.PurchaseOrderId, cancellationToken);

        if (updateResult)
        {
            _logger.LogInformation($"PurchaseOrderId: {payload.PurchaseOrderId}, marked as shipped.");
        }
        else
        {
            _logger.LogInformation($"Failed marking PurchaseOrderId: {payload.PurchaseOrderId} as shipped.");
        }
    }
}
