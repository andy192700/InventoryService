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
    public string TopicName => "PurchaseOrderCreated";

    /// <inheritdoc/>
    public string QueueName => "PurchaseOrderCreated";
}

/// <summary>
/// A consumer that consumes purchase orders.
/// </summary>
public sealed class PurchaseOrderConsumer : SQSConsumer<FulfilPurchaseOrderRequest, PurchaseOrderConsumerDescriptor>
{
    private readonly ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic> _producer;

    private readonly Random _random = new();

    public PurchaseOrderConsumer(
        ISQSSource<PurchaseOrderConsumerDescriptor> source,
        ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic> producer,
        ILogger<PurchaseOrderConsumer> logger)
        : base(source, logger)
    {
        _producer = producer;
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(FulfilPurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has received a new Purchase Order, id: {payload.PurchaseOrderId}");

        int preparationTimeSeconds = _random.Next(10, 51);

        _logger.LogInformation($"Team is working hard to fulfill the purchase order. Estimated wait time: {preparationTimeSeconds} seconds.");

        await Task.Delay(TimeSpan.FromSeconds(preparationTimeSeconds), cancellationToken);

        await _producer.ProduceAsync(new ShipPurchaseOrderRequest(payload.PurchaseOrderId), cancellationToken);
    }
}
