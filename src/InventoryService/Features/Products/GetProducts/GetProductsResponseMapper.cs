namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Maps a database response to a products response.
/// </summary>
public interface IGetProductsResponseMapper
{
    /// <summary>
    /// Maps the database response to a product response.
    /// </summary>
    /// <param name="getProductsDBProductsResponse">The database response to map.</param>
    /// <returns>A product response.</returns>
    ICollection<GetProductsResponse> Map(GetProductsDBResponse getProductsDBProductsResponse);
}

/// <summary>
/// Maps a database response to a products response.
/// </summary>
public class GetProductsResponseMapper : IGetProductsResponseMapper
{
    /// <inheritdoc/>
    public ICollection<GetProductsResponse> Map(GetProductsDBResponse getProductsDBResponse)
    {
        var productCountries = new Dictionary<int, List<GetProductsCountriesResponse>>();

        foreach (var product in getProductsDBResponse.Products)
        {
            var items = getProductsDBResponse.ProductCountries.Where(x => x.ProductId == product.ProductId);

            var productCountryCollection = new List<GetProductsCountriesResponse>();

            foreach (var productCountry in items)
            {
                productCountryCollection.Add(new GetProductsCountriesResponse
                (
                    CountryId: productCountry.CountryId,
                    UnitPrice: (double)productCountry.UnitPrice
                ));
            }

            productCountries.TryAdd(product.ProductId, productCountryCollection);
        }

        var products = new List<GetProductsResponse>();

        foreach (var product in getProductsDBResponse.Products)
        {
            products.Add(new GetProductsResponse
            (
                ProductId: product.ProductId,
                CreatedDate: product.CreatedDate,
                ProductName: product.ProductName,
                ProductDesc: product.ProductDesc,
                ProductCountries: productCountries[product.ProductId]
            ));
        }

        return products;
    }
}
