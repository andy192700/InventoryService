using Messaging;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Topic for producing notifications of created purchase orders
/// </summary>
public sealed class CreatePurchaseOrderTopic : ITopicDescriptor
{
    /// <inheritdoc/>
    public string TopicName => "CreatePurchaseOrder";
}
