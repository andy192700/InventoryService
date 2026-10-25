namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public interface IGetPurchaseOrderResponseMapper
{
    GetPurchaseOrderResponse Map(GetPurchaseOrderDBResponse dbResponse);
}

public class GetPurchaseOrderResponseMapper : IGetPurchaseOrderResponseMapper
{
    public GetPurchaseOrderResponse Map(GetPurchaseOrderDBResponse dbResponse)
    {
        var lines = new List<GetPurchaseOrderLineResponse>();
    
        foreach (var line in dbResponse.lines!)
        {
            lines.Add(new GetPurchaseOrderLineResponse
            {
                PurchaseOrderLineId = line.PurchaseOrderLineId,
                ProductId = line.ProductId,
                OrderQuantity = line.OrderQuantity,
                RowNo = line.RowNo
            });
        }

        return new GetPurchaseOrderResponse
        {
            PurchaseOrderId = dbResponse.header!.PurchaseOrderId,
            OrderDate = dbResponse.header.OrderDate,
            CustomerId = dbResponse.header.CustomerId,
            CountryId = dbResponse.header.CountryId,
            PurchaseOrderLines = lines
        };
    }
}
