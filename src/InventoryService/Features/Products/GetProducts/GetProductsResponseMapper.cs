namespace InventoryService.Features.Products.GetProducts;

public interface IGetProductsResponseMapper
{
    ICollection<GetProductsResponse> Map(GetProductsDBResponse getProductsDBProductsResponse);
}

public class GetProductsResponseMapper : IGetProductsResponseMapper
{
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
