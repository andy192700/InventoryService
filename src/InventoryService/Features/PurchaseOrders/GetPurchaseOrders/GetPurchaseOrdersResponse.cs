namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A Purchase Order response.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order Id.</param>
/// <param name="OrderDate">The date and time when the order was created.</param>
/// <param name="CustomerId">The Customer Id.</param>
/// <param name="CountryId">The Country Id.</param>
/// <param name="PurchaseOrderLines">A collection of purchase order lines.</param>
public record GetPurchaseOrdersResponse
(
    int PurchaseOrderId,
    DateTimeOffset OrderDate,
    int CustomerId,
    int CountryId,
    ICollection<GetPurchaseOrdersLineResponse> PurchaseOrderLines
);

/// <summary>
/// A purchase order line.
/// </summary>
/// <param name="PurchaseOrderLineId">The Purchase Order Line Id.</param>
/// <param name="ProductId">The Product Id.</param>
/// <param name="OrderQuantity">The Order Quantity.</param>
/// <param name="RowNo">The Row Number.</param>
public record GetPurchaseOrdersLineResponse
(
    int PurchaseOrderLineId,
    int ProductId,
    int OrderQuantity,
    int RowNo
);
