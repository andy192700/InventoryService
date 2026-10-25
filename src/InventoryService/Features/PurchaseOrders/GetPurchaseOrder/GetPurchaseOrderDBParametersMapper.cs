namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public interface IGetPurchaseOrderDBParametersMapper
{
    GetPurchaseOrderDBParameters Map(GetPurchaseOrderRequest request);
}

public class GetPurchaseOrderDBParametersMapper : IGetPurchaseOrderDBParametersMapper
{
    public GetPurchaseOrderDBParameters Map(GetPurchaseOrderRequest request)
    {
        return new GetPurchaseOrderDBParameters
        {
            PurchaseOrderId = int.Parse(request.PurchaseOrderId!)
        };
    }
}
