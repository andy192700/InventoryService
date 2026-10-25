namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// A Purchase Order response.
/// </summary>
public class GetPurchaseOrderResponse
{
    /// <summary>
    /// The Purchase Order Id.
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// The date and time when the order was created.
    /// </summary>
    public DateTimeOffset OrderDate { get; set; }

    /// <summary>
    /// The Customer Id.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The Country Id.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// A collection of purchase order lines.
    /// </summary>
    public required ICollection<GetPurchaseOrderLineResponse> PurchaseOrderLines { get; set; }
}

/// <summary>
/// A purchase order line.
/// </summary>
public class GetPurchaseOrderLineResponse
{
    /// <summary>
    /// The Purchase Order Line Id.
    /// </summary>
    public int PurchaseOrderLineId { get; set; }

    /// <summary>
    /// The Product Id.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The Order Quantity.
    /// </summary>
    public int OrderQuantity { get; set; }

    /// <summary>
    /// The Row Number.
    /// </summary>
    public int RowNo { get; set; }
}