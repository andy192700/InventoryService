namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// Database parameters for retrieving a purchase order.
/// </summary>
public class GetPurchaseOrderDBParameters
{
    /// <summary>
    /// THe Purchase Order's id.
    /// </summary>
    public int PurchaseOrderId { get; set; }
}
