using Messaging;

namespace WarehouseService;

/// <summary>
/// A record recieved by the warehouse when a new purchase order is created.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order Id.</param>
public record FulfilPurchaseOrderRequest
(
    int PurchaseOrderId
);

/// <summary>
/// Describes the consumer for consuming newly created purchase orders.
/// </summary>
public sealed class PurchaseOrderConsumerDescriptor : IQueueDescriptor
{
    /// <inheritdoc/>
    public string TopicName => "CreatePurchaseOrder";

    /// <inheritdoc/>
    public string QueueName => "CreatePurchaseOrder";
}

/// <summary>
/// A consumer that consumes purchase orders.
/// </summary>
public sealed class PurchaseOrderConsumer : SQSConsumer<FulfilPurchaseOrderRequest, PurchaseOrderConsumerDescriptor>
{
    public PurchaseOrderConsumer(
        ISQSSource<PurchaseOrderConsumerDescriptor> source,
        ILogger<PurchaseOrderConsumer> logger)
        : base(source, logger)
    {
    }

    /// <inheritdoc/>
    protected override Task HandleAsync(FulfilPurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has received a new Purchase Order, id: {payload.PurchaseOrderId}");

        return Task.CompletedTask;
    }
}
