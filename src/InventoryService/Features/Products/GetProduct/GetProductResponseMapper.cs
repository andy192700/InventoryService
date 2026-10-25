namespace InventoryService.Features.Products.GetProduct;

public interface IGetProductResponseMapper
{
    GetProductResponse Map(GetProductDBResponse getProductsDBProductResponse);
}

public class GetProductResponseMapper : IGetProductResponseMapper
{
    public GetProductResponse Map(GetProductDBResponse getProductDBResponse)
    {
        var product = new GetProductResponse
        {
            ProductId = getProductDBResponse.Product!.ProductId,
            CreatedDate = getProductDBResponse.Product.CreatedDate,
            ProductName = getProductDBResponse.Product.ProductName,
            ProductDesc = getProductDBResponse.Product.ProductDesc
        };

        foreach (var productCountry in getProductDBResponse.ProductCountries)
        {
            product.ProductCountries.Add(new GetProductCountriesResponse
            {
                CountryId = productCountry.CountryId,
                UnitPrice = productCountry.UnitPrice
            });
        }

        return product;
    }
}