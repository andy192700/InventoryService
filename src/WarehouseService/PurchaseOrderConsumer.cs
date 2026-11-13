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
public sealed class PurchaseOrderConsumer : SQSConsumer<FulfilPurchaseOrderRequest>
{
    private readonly ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic> _producer;

    private readonly ILogger<PurchaseOrderConsumer> _logger;

    private readonly Random _random = new();

    public PurchaseOrderConsumer(
        ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic> producer,
        ILogger<PurchaseOrderConsumer> logger)
    {
        _producer = producer;

        _logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task ConsumeAsync(FulfilPurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has received a new Purchase Order, id: {payload.PurchaseOrderId}");

        int preparationTimeSeconds = _random.Next(10, 51);

        _logger.LogInformation($"Team is working hard to fulfill the purchase order. Estimated wait time: {preparationTimeSeconds} seconds.");

        await Task.Delay(TimeSpan.FromSeconds(preparationTimeSeconds), cancellationToken);

        await _producer.ProduceAsync(new ShipPurchaseOrderRequest(payload.PurchaseOrderId), cancellationToken);
    }
}
