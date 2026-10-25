namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersDBResponse
{
    public required ICollection<GetPurchaseOrdersDBResponseHeader> Headers { get; set; }

    public required ICollection<GetPurchaseOrdersDBResponseLine> Lines { get; set; }
}

public class GetPurchaseOrdersDBResponseHeader
{
    public int PurchaseOrderId { get; set; }

    public DateTimeOffset OrderDate { get; set; }

    public int CustomerId { get; set; }

    public int CountryId { get; set; }
}

public class GetPurchaseOrdersDBResponseLine
{
    public int PurchaseOrderId { get; set; }

    public int PurchaseOrderLineId { get; set; }

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public int RowNo { get; set; }
}