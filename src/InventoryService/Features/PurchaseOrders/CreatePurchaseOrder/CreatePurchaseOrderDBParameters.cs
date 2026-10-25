using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Database parameters for creating a purchase order.
/// </summary>
public class CreatePurchaseOrderDBParameters
{
    /// <summary>
    /// The Purchase Order's CustomerId.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The Purchase Order's CountryId.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// <see cref="DataTable"/> containing the Purchase Order's lines.
    /// </summary>
    public required DataTable Lines { get; set; }
}