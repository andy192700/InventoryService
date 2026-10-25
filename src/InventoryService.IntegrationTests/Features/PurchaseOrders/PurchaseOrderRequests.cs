namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

public class CreatePurchaseOrderIntRequest
{
    public string? CustomerId { get; set; }

    public string? CountryId { get; set; }

    public ICollection<CreatePurchaseOrderIntRequestLine>? lines { get; set; }
}

public class CreatePurchaseOrderIntRequestLine
{
    public string? ProductId { get; set; }

    public string? OrderQuantity { get; set; }
}