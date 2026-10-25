namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderResponse
{
    public int PurchaseOrderId { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    public int CustomerId { get; set; }

    public int CountryId { get; set; }

    public required ICollection<GetPurchaseOrderLineResponse> PurchaseOrderLines { get; set; }
}

public class GetPurchaseOrderLineResponse
{
    public int PurchaseOrderLineId { get; set; }

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public int RowNo { get; set; }
}