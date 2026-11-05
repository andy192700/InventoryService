namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A purchase orders database response.
/// </summary>
/// <param name="Headers">Header components of the database response.</param>
/// <param name="Lines">Line components of the database response.</param>
public record GetPurchaseOrdersDBResponse
(
    ICollection<GetPurchaseOrdersDBResponseHeader> Headers,
    ICollection<GetPurchaseOrdersDBResponseLine> Lines
);

/// <summary>
/// Header component of the database response.
/// </summary>
/// <param name="PurchaseOrderId">THe Purchase Order's Id.</param>
/// <param name="OrderDate">The date and time when the order was made.</param>
/// <param name="CustomerId">The Purchase Order's CustomerId.</param>
/// <param name="CountryId">The Purchase Order's CountyId.</param>
public record GetPurchaseOrdersDBResponseHeader
(
    int PurchaseOrderId,
    DateTimeOffset OrderDate,
    int CustomerId,
    int CountryId
);

/// <summary>
/// Line components of the database response.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order Id.</param>
/// <param name="PurchaseOrderLineId">The Purchase Order Line Id.</param>
/// <param name="ProductId">The Product Id.</param>
/// <param name="OrderQuantity">The Order Quantity.</param>
/// <param name="RowNo">The Row Number.</param>
public record GetPurchaseOrdersDBResponseLine
(
    int PurchaseOrderId,
    int PurchaseOrderLineId,
    int ProductId,
    int OrderQuantity,
    long RowNo
);