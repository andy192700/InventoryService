namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderDBResponse
{
    public GetPurchaseOrderDBResponseHeader? header { get; set; }

    public ICollection<GetPurchaseOrderDBResponseLine>? lines { get; set; }
}

public class GetPurchaseOrderDBResponseHeader
{
    public int PurchaseOrderId { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    public int CustomerId { get; set; }

    public int CountryId { get; set; }
}

public class GetPurchaseOrderDBResponseLine
{
    public int PurchaseOrderLineId { get; set; }

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public int RowNo { get; set; }
}