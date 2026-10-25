using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

public interface ICreateProductDBParametersMapper
{
    CreateProductDBParameters Map(CreateProductRequest request);
}

public class CreateProductDBParametersMapper : ICreateProductDBParametersMapper
{
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
