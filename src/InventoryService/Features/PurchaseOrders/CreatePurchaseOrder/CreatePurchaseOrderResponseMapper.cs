using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Maps a create purchase order database response to a handler response.
/// </summary>
public interface ICreatePurchaseOrderResponseMapper
{
    /// <summary>
    /// Maps the database response to a <see cref="CreatePurchaseOrderResponse"/> instance.
    /// </summary>
    /// <param name="dbResponse">The database response containing purchase order data.</param>
    /// <returns>A <see cref="CreatePurchaseOrderResponse"/> with mapped data.</returns>
    CreatePurchaseOrderResponse Map(CreatePurchaseOrderDBResponse dbResponse);
}

/// <summary>
/// Maps a create purchase order database response to a handler response.
/// </summary>
public class CreatePurchaseOrderResponseMapper : ICreatePurchaseOrderResponseMapper
{
    /// <inheritdoc />
    public CreatePurchaseOrderResponse Map(CreatePurchaseOrderDBResponse dbResponse)
    {
        return new CreatePurchaseOrderResponse
        {
            PurchaseOrderId = dbResponse.PurchaseOrderId
        };
    }
}
