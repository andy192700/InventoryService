namespace InventoryService.Features.Products.GetProduct;

public class GetProductDBResponse
{
    public GetProductDBProductResponse? Product { get; set; }

    public ICollection<GetProductDbProductCountriesResponse> ProductCountries { get; set; } = [];
}

public class GetProductDBProductResponse
{
    public int ProductId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductDesc { get; set; } = string.Empty;
}

public class GetProductDbProductCountriesResponse
{
    public int CountryId { get; set; }

    public double UnitPrice { get; set; }
}