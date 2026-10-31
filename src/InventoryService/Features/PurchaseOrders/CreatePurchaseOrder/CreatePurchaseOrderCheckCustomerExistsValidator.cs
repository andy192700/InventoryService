using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Data validator, validating that a supplied Customer Id does exist.
/// </summary>
public class CreatePurchaseOrderCheckCustomerExistsValidator : DataIntegrityValidator<CreatePurchaseOrderRequest>
{
    /// <inheritdoc/>
    protected override string ErrorMessage => "A Customer with the requested CustomerId does not exist.";

    /// <inheritdoc/>
    protected override string StoredProcedureName => "warehouse.usp_CheckCustomerExists";

    /// <inheritdoc/>
    protected override string[] ApplicableProperties => [nameof(CreatePurchaseOrderRequest.Payload.CustomerId)];

    public CreatePurchaseOrderCheckCustomerExistsValidator(ISQLActivity activity) : base(activity) { }

    /// <inheritdoc/>
    protected override object CreateParameters(CreatePurchaseOrderRequest request)
    {
        return new
        {
            request.Payload.CustomerId
        };
    }
}
