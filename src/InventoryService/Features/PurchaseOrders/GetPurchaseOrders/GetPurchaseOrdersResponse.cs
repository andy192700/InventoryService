namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersResponse
{
    public int PurchaseOrderId { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    public int CustomerId { get; set; }

    public int CountryId { get; set; }

    public required ICollection<GetPurchaseOrdersLineResponse> PurchaseOrderLines { get; set; }
}

public class GetPurchaseOrdersLineResponse
{
    public int PurchaseOrderLineId { get; set; }

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public int RowNo { get; set; }
}
