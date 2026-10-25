namespace InventoryService.Features.Products.GetProducts;

public class GetProductsResponse
{
    public int ProductId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductDesc { get; set; } = string.Empty;

    public ICollection<GetProductCountriesResponse> ProductCountries { get; set; } = [];
}

public class GetProductCountriesResponse
{
    public int CountryId { get; set; }

    public double UnitPrice { get; set; }
}
