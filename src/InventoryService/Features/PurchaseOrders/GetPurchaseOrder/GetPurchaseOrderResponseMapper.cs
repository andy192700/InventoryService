namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// A purchase Order response mapper.
/// </summary>
public interface IGetPurchaseOrderResponseMapper
{
    /// <summary>
    /// Map the database response to a handler response.
    /// </summary>
    /// <param name="dbResponse">The database response.</param>
    /// <returns>The handler response.</returns>
    GetPurchaseOrderResponse Map(GetPurchaseOrderDBResponse dbResponse);
}

/// <summary>
/// A purchase Order response mapper.
/// </summary>
public class GetPurchaseOrderResponseMapper : IGetPurchaseOrderResponseMapper
{
    /// <inheritdoc/>
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
