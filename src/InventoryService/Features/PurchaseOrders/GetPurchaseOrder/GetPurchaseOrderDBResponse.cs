namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// A purchase order database response.
/// </summary>
/// <param name="header">Header component of the database response.</param>
/// <param name="lines">Line components of the database response.</param>
public record GetPurchaseOrderDBResponse
(
    GetPurchaseOrderDBResponseHeader? header,
    ICollection<GetPurchaseOrderDBResponseLine>? lines
);

/// <summary>
/// Header component of the database response.
/// </summary>
/// <param name="PurchaseOrderId">THe Purchase Order's Id.</param>
/// <param name="OrderDate">The date and time when the order was made.</param>
/// <param name="CustomerId">The Purchase Order's CustomerId.</param>
/// <param name="CountryId">The Purchase Order's CountyId.</param>
/// <param name="Status">The Purchase Order's Status.</param>
public record GetPurchaseOrderDBResponseHeader
(
    int PurchaseOrderId,
    DateTimeOffset OrderDate,
    int CustomerId,
    int CountryId,
    string Status
);

/// <summary>
/// Line components of the database response.
/// </summary>
/// <param name="PurchaseOrderLineId">The Purchase Order Line Id.</param>
/// <param name="ProductId">The Product Id.</param>
/// <param name="OrderQuantity">The Order Quantity.</param>
/// <param name="RowNo">The Row Number.</param>
public record GetPurchaseOrderDBResponseLine
(
    int PurchaseOrderLineId,
    int ProductId,
    int OrderQuantity,
    long RowNo
);