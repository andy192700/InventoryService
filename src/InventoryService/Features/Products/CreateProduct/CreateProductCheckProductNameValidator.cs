using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Data validator, checks that a Product with the same name as that in the incoming request does not exist.
/// </summary>
public class CreateProductCheckProductNameValidator : DataIntegrityValidator<CreateProductRequest>
{
    /// <inheritdoc/>
    protected override string ErrorMessage => "A Product with the requested Product Name already exists.";

    /// <inheritdoc/>
    protected override string StoredProcedureName => "warehouse.usp_CheckProductName";

    /// <inheritdoc/>
    protected override string[] ApplicableProperties => [ nameof(CreateProductRequest.Payload.ProductName) ];

    public CreateProductCheckProductNameValidator(ISQLActivity activity) : base(activity) { }

    /// <inheritdoc/>
    protected override object CreateParameters(CreateProductRequest request)
    {
        return new
        {
            request.Payload.ProductName
        };
    }
}
