namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// List prchase orders database parameters.
/// </summary>
/// <param name="ProductId">The Product Id.</param>
/// <param name="CustomerId">The Customer Id.</param>
/// <param name="CountryId">The Country Id.</param>
public record GetPurchaseOrdersDBParameters
(
    int? ProductId,
    int? CustomerId,
    int? CountryId
);
