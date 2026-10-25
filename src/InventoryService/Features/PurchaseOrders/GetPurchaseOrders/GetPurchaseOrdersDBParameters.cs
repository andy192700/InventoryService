namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// List prchase orders database parameters.
/// </summary>
public class GetPurchaseOrdersDBParameters
{
    /// <summary>
    /// The Product Id.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// The Customer Id.
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// The Country Id.
    /// </summary>
    public int? CountryId { get; set; }
}
