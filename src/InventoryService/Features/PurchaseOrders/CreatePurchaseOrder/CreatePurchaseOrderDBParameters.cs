using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

public class CreatePurchaseOrderDBParameters
{
    public int CustomerId { get; set; }

    public int CountryId { get; set; }

    public required DataTable Lines { get; set; }
}