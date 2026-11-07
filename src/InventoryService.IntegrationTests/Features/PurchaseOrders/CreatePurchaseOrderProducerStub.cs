using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using Messaging;

namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

/// <summary>
/// Stubs out SNS producer comms when creating purchase orders
/// </summary>
public class CreatePurchaseOrderProducerStub : ISNSProducer<CreatePurchaseOrderResponse, CreatePurchaseOrderTopic>
{
    public Task ProduceAsync(CreatePurchaseOrderResponse message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
