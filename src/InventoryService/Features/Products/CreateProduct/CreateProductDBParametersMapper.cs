using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Maps a product request to database parameters.
/// </summary>
public interface ICreateProductDBParametersMapper
{
    /// <summary>
    /// Maps the product request to database parameters.
    /// </summary>
    /// <param name="request">The product request to map.</param>
    /// <returns>Database parameters.</returns>
    CreateProductDBParameters Map(CreateProductRequest request);
}

/// <summary>
/// Maps a product request to database parameters.
/// </summary>
public class CreateProductDBParametersMapper : ICreateProductDBParametersMapper
{
    /// <inheritdoc />
    public CreateProductDBParameters Map(CreateProductRequest request)
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

        return new CreateProductDBParameters
        {
            ProductName = request.Payload.ProductName!,
            ProductDesc = request.Payload.ProductDesc!,
            ProductCountries = productCountriesdt
        };
    }
}