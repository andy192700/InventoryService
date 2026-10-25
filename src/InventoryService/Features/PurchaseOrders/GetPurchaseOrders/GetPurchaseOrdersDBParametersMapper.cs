namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// Maps a Purchase order request to database parameters.
/// </summary>
public interface IGetPurchaseOrdersDBParametersMapper
{
    /// <summary>
    /// Maps the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The database parameters.</returns>
    GetPurchaseOrdersDBParameters Map(GetPurchaseOrdersRequest request);
}

/// <summary>
/// Maps a Purchase order request to database parameters.
/// </summary>
public class GetPurchaseOrdersDBParametersMapper : IGetPurchaseOrdersDBParametersMapper
{
    /// <inheritdoc/>
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
