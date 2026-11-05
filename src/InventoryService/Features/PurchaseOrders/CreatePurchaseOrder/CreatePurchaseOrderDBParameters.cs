using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Database parameters for creating a purchase order.
/// </summary>
/// <param name="CustomerId">The Purchase Order's CustomerId.</param>
/// <param name="CountryId">The Purchase Order's CountryId.</param>
/// <param name="Lines"><see cref="DataTable"/> containing the Purchase Order's lines.</param>
public record CreatePurchaseOrderDBParameters
(
    int CustomerId,
    int CountryId,
    DataTable Lines
);