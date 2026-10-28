using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.Products.CreateProduct;

public class CreateProductDataIntegrityValidator : DataIntegrityValidator<CreateProductRequest>
{
    protected override string ErrorMessage => "A Product with the requested Product Name already exists.";

    protected override string StoredProcedureName => "warehouse.usp_CheckProductName";

    protected override string[] ApplicableProperties => [ "ProductName" ];

    public CreateProductDataIntegrityValidator(ISQLConnectionProvider connectionProvider) : base(connectionProvider) { }
    
    protected override object CreateParameters(CreateProductRequest request)
    {
        return new
        {
            request.Payload.ProductName
        };
    }
}
