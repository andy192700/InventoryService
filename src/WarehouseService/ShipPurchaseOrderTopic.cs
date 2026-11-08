using Messaging;

namespace WarehouseService;

/// <summary>
/// Topic for notifying that a particular purchase order is ready to be shipped.
/// </summary>
public class ShipPurchaseOrderTopic : ITopicDescriptor
{
    /// <inheritdoc/>
    public string TopicName => "ShipPurchaseOrder";
}

/// <summary>
/// The request payload for a ready to be shipped purchase order.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order Id.</param>
public record ShipPurchaseOrderRequest
(
    int PurchaseOrderId
);
