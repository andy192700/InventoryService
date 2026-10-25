namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A purchase Orders response mapper.
/// </summary>
public interface IGetPurchaseOrdersResponseMapper
{
    /// <summary>
    /// Map the database response to a handler response.
    /// </summary>
    /// <param name="dbResponse">The database response.</param>
    /// <returns>The handler response.</returns>
    ICollection<GetPurchaseOrdersResponse> Map(GetPurchaseOrdersDBResponse dbResponse);
}

/// <summary>
/// A purchase Orders response mapper.
/// </summary>
public class GetPurchaseOrdersResponseMapper : IGetPurchaseOrdersResponseMapper
{
    /// <inheritdoc/>
    public ICollection<GetPurchaseOrdersResponse> Map(GetPurchaseOrdersDBResponse dbResponse)
    {
        var orders = new List<GetPurchaseOrdersResponse>();

        foreach (var order in dbResponse.Headers)
        {
            orders.Add(new GetPurchaseOrdersResponse
            {
                PurchaseOrderId = order.PurchaseOrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CountryId = order.CountryId,
                PurchaseOrderLines = []
            });
        }

        foreach (var order in orders)
        {
            var lines = dbResponse.Lines.Where(x => x.PurchaseOrderId == order.PurchaseOrderId).ToList();

            foreach (var line in lines)
            {
                order.PurchaseOrderLines.Add(new GetPurchaseOrdersLineResponse
                {
                    PurchaseOrderLineId = line.PurchaseOrderLineId,
                    ProductId = line.ProductId,
                    OrderQuantity = line.OrderQuantity,
                    RowNo = line.RowNo
                });
            }
        }

        return orders;
    }
}
