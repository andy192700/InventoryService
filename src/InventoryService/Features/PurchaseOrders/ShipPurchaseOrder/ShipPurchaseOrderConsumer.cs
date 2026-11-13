using Messaging;

namespace InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;

/// <summary>
/// Payload for the <see cref="ShipPurchaseOrderConsumer"/>.
/// </summary>
/// <param name="PurchaseOrderId">The purchase order id, to be marked as shipped.</param>
public record ShipPurchaseOrderRequest
(
    int PurchaseOrderId
);

/// <summary>
/// The queue descriptor for the <see cref="ShipPurchaseOrderConsumer"/>.
/// </summary>
public class ShipPurchaseOrderDescriptor : IQueueDescriptor
{
    /// <inheritdoc/>
    public string TopicName => "ShipPurchaseOrder";

    /// <inheritdoc/>
    public string QueueName => "ShipPurchaseOrder";
}

/// <summary>
/// Consumer whose purpose is mark Purchase orders as shipped, sent by the warehouse after the team has finished preparing the order.
/// </summary>
public class ShipPurchaseOrderConsumer : SQSConsumer<ShipPurchaseOrderRequest>
{
    private readonly IShipPurchaseOrderDBCommand _dbCommand;

    private readonly ILogger<ShipPurchaseOrderConsumer> _logger;

    public ShipPurchaseOrderConsumer(
        IShipPurchaseOrderDBCommand dbCommand,
        ILogger<ShipPurchaseOrderConsumer> logger)
    {
        _dbCommand = dbCommand;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task ConsumeAsync(ShipPurchaseOrderRequest payload, CancellationToken cancellationToken)
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
