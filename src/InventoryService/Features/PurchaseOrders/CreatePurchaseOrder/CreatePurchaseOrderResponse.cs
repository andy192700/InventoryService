namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// A Create purchase order response.
/// </summary>
/// <param name="PurchaseOrderId">Gets or sets the identifier of the created purchase order.</param>
public record CreatePurchaseOrderResponse
(
    int PurchaseOrderId
);
