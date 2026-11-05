namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// A database purchase order response.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order's Id.</param>
public record CreatePurchaseOrderDBResponse
(
    int PurchaseOrderId
);
