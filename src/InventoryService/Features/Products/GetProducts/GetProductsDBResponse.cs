namespace InventoryService.Features.Products.GetProducts;

public class GetProductsDBResponse
{
    public ICollection<GetProductsDBProductsResponse> Products { get; set; } = [];

    public ICollection<GetProductsDbProductCountriesResponse> ProductCountries { get; set; } = [];
}

public class GetProductsDBProductsResponse
{
    public int ProductId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductDesc { get; set; } = string.Empty;
}

public class GetProductsDbProductCountriesResponse
{
    public int ProductId { get; set; }

    public int CountryId { get; set; }

    public double UnitPrice { get; set; }
}