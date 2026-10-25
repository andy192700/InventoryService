namespace InventoryService.IntegrationTests.Features.Products;

public class CreateProductIntRequest
{
    public string? ProductName { get; set; }

    public string? ProductDesc { get; set; }

    public ICollection<CreateProductIntRequestProductCountries>? ProductCountries { get; set; }
}

public class CreateProductIntRequestProductCountries
{
    public string? CountryId { get; set; }

    public string? UnitPrice { get; set; }
}