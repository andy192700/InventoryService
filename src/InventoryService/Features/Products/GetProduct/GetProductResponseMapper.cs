namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// Maps a database response to a product response.
/// </summary>
public interface IGetProductResponseMapper
{
    /// <summary>
    /// Maps the database response to a product response.
    /// </summary>
    /// <param name="getProductsDBProductResponse">The database response to map.</param>
    /// <returns>A product response.</returns>
    GetProductResponse Map(GetProductDBResponse getProductsDBProductResponse);
}

/// <summary>
/// Maps a database response to a product response.
/// </summary>
public class GetProductResponseMapper : IGetProductResponseMapper
{
    /// <inheritdoc/>
    public GetProductResponse Map(GetProductDBResponse getProductDBResponse)
    {
        var productCountries = new List<GetProductCountriesResponse>();

        foreach (var productCountry in getProductDBResponse.ProductCountries)
        {
            productCountries.Add(new GetProductCountriesResponse
            (
                CountryId: productCountry.CountryId,
                UnitPrice: productCountry.UnitPrice
            ));
        }

        var product = new GetProductResponse
        (
            ProductId: getProductDBResponse.Product!.ProductId,
            CreatedDate: getProductDBResponse.Product.CreatedDate,
            ProductName: getProductDBResponse.Product.ProductName,
            ProductDesc: getProductDBResponse.Product.ProductDesc,
            ProductCountries: productCountries
        );

        return product;
    }
}