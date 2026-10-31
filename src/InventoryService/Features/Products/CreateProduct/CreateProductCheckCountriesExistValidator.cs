using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;
using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Data validator, checks that all supplied countries exist.
/// </summary>
public class CreateProductCheckCountriesExistValidator : DataIntegrityValidator<CreateProductRequest>
{
    /// <inheritdoc/>
    protected override string ErrorMessage => "One or more of the requested Countries does not exist.";

    /// <inheritdoc/>
    protected override string StoredProcedureName => "warehouse.usp_CheckProductCountriesExist";

    /// <inheritdoc/>
    protected override string[] ApplicableProperties => [ nameof(CreateProductRequestProductCountries.CountryId)];

    public CreateProductCheckCountriesExistValidator(ISQLActivity activity) : base(activity) { }

    /// <inheritdoc/>
    protected override object CreateParameters(CreateProductRequest request)
    {
        var productCountriesdt = new DataTable();

        productCountriesdt.Columns.Add("CountryId", typeof(int));
        productCountriesdt.Columns.Add("UnitPrice", typeof(double));

        foreach (var country in request.Payload.ProductCountries!)
        {
            var row = productCountriesdt.NewRow();

            row.SetField("CountryId", country.CountryId);
            row.SetField("UnitPrice", country.UnitPrice);

            productCountriesdt.Rows.Add(row);
        }

        return new
        {
            ProductCountries = productCountriesdt
        };
    }
}
