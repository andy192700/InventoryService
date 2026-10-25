namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A purchase orders database response.
/// </summary>
public class GetPurchaseOrdersDBResponse
{
    /// <summary>
    /// Header components of the database response.
    /// </summary>
    public required ICollection<GetPurchaseOrdersDBResponseHeader> Headers { get; set; }

    /// <summary>
    /// Line components of the database response.
    /// </summary>
    public required ICollection<GetPurchaseOrdersDBResponseLine> Lines { get; set; }
}

/// <summary>
/// Header component of the database response.
/// </summary>
public class GetPurchaseOrdersDBResponseHeader
{
    /// <summary>
    /// THe Purchase Order's Id.
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// The date and time when the order was made.
    /// </summary>
    public DateTimeOffset OrderDate { get; set; }

    /// <summary>
    /// The Purchase Order's CustomerId.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The Purchase Order's CountyId.
    /// </summary>
    public int CountryId { get; set; }
}

/// <summary>
/// Line components of the database response.
/// </summary>
public class GetPurchaseOrdersDBResponseLine
{
    /// <summary>
    /// The Purchase Order Id.
    /// </summary>
    public int PurchaseOrderId { get; set; }

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