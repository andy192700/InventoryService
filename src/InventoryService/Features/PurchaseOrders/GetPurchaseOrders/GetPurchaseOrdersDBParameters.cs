namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersDBParameters
{
    public int? ProductId { get; set; }

    public int? CustomerId { get; set; }

    public int? CountryId { get; set; }
}
