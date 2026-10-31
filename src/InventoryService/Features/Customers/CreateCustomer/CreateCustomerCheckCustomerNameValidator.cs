using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// Data validator, checks that a Customer with the same name as that in the incoming request does not exist.
/// </summary>
public class CreateCustomerCheckCustomerNameValidator : DataIntegrityValidator<CreateCustomerRequest>
{
    /// <inheritdoc/>
    protected override string ErrorMessage => "A Customer with the requested full name already exists.";

    /// <inheritdoc/>
    protected override string StoredProcedureName => "warehouse.usp_CheckCustomerName";

    /// <inheritdoc/>
    protected override string[] ApplicableProperties => [ nameof(CreateCustomerRequest.Payload.FirstName), nameof(CreateCustomerRequest.Payload.LastName) ];

    public CreateCustomerCheckCustomerNameValidator(ISQLActivity activity) : base(activity) { }

    /// <inheritdoc/>
    protected override object CreateParameters(CreateCustomerRequest request)
    {
        return new
        {
            request.Payload.FirstName,
            request.Payload.LastName
        };
    }
}
