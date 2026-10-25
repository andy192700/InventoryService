namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

public interface ICreatePurchaseOrderResponseMapper
{
    CreatePurchaseOrderResponse Map(CreatePurchaseOrderDBResponse dbResponse);
}

public class CreatePurchaseOrderResponseMapper : ICreatePurchaseOrderResponseMapper
{
    public CreatePurchaseOrderResponse Map(CreatePurchaseOrderDBResponse dbResponse)
    {
        return new CreatePurchaseOrderResponse
        {
            PurchaseOrderId = dbResponse.PurchaseOrderId
        };
    }
}
