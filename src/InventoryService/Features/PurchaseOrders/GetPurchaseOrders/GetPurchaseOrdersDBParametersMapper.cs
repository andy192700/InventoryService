namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public interface IGetPurchaseOrdersDBParametersMapper
{
    GetPurchaseOrdersDBParameters Map(GetPurchaseOrdersRequest request);
}

public class GetPurchaseOrdersDBParametersMapper : IGetPurchaseOrdersDBParametersMapper
{
    public GetPurchaseOrdersDBParameters Map(GetPurchaseOrdersRequest request)
    {
        return new GetPurchaseOrdersDBParameters
        {
            ProductId = request.ProductId is not null ? int.Parse(request.ProductId!) : default,
            CountryId = request.CountryId is not null ? int.Parse(request.CountryId!) : default,
            CustomerId = request.CustomerId is not null ? int.Parse(request.CustomerId!) : default
        };
    }
}
