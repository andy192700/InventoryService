using Amazon.SQS.Model;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;
using Messaging;
using System.Runtime.CompilerServices;

namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

/// <summary>
/// Stubs out SNS producer comms when creating purchase orders
/// </summary>
public class CreatePurchaseOrderProducerStub : ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>
{
    public Task ProduceAsync(CreatePurchaseOrderResponse message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}