namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// Database parameters for retrieving a purchase order.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order's id.</param>
public record GetPurchaseOrderDBParameters
(
    int PurchaseOrderId
);
