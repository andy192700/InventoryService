namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// Maps a purchase order request to database parameters.
/// </summary>
public interface IGetPurchaseOrderDBParametersMapper
{
    /// <summary>
    /// Map the request to database parameters.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The database parameters.</returns>
    GetPurchaseOrderDBParameters Map(GetPurchaseOrderRequest request);
}

/// <summary>
/// Maps a purchase order request to database parameters.
/// </summary>
public class GetPurchaseOrderDBParametersMapper : IGetPurchaseOrderDBParametersMapper
{
    /// <inheritdoc/>
    public GetPurchaseOrderDBParameters Map(GetPurchaseOrderRequest request)
    {
        return new GetPurchaseOrderDBParameters
        {
            PurchaseOrderId = int.Parse(request.PurchaseOrderId!)
        };
    }
}
