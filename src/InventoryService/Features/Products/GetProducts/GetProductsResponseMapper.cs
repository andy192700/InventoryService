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
        var products = new List<GetProductsResponse>();

        foreach (var product in getProductsDBResponse.Products)
        {
            products.Add(new GetProductsResponse
            {
                ProductId = product.ProductId,
                CreatedDate = product.CreatedDate,
                ProductName = product.ProductName,
                ProductDesc = product.ProductDesc
            });
        }

        foreach (var product in products)
        {
            var productCountries = getProductsDBResponse.ProductCountries.Where(x => x.ProductId == product.ProductId);

            foreach (var productCountry in productCountries)
            {
                product.ProductCountries.Add(new GetProductCountriesResponse
                {
                    CountryId = productCountry.CountryId,
                    UnitPrice = productCountry.UnitPrice
                });
            }
        }

        return products;
    }
}
