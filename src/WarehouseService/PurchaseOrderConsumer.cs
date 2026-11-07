using Amazon.SQS.Model;
using Messaging;

namespace WarehouseService;

public record PurchaseOrderRequest
(
    int PurchaseOrderId
);

public sealed class PurchaseOrderConsumerDescriptor : IQueueDescriptor
{
    public string TopicName => "NewPurchaseOrder";

    public string QueueName => "NewPurchaseOrder";
}

public sealed class PurchaseOrderConsumer : SQSConsumer<PurchaseOrderRequest, PurchaseOrderConsumerDescriptor>
{
    public PurchaseOrderConsumer(
        ISQSSource<PurchaseOrderConsumerDescriptor> source,
        ILogger<PurchaseOrderConsumer> logger)
        : base(source, logger)
    {
    }

    protected override Task HandleAsync(PurchaseOrderRequest payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Warehouse has received a new Purchase Order, id: {payload.PurchaseOrderId}");

        return Task.CompletedTask;
    }
}
