namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// A database products response.
/// </summary>
public class GetProductsDBResponse
{
    /// <summary>
    /// The list of products, retrieved from the database.
    /// </summary>
    public ICollection<GetProductsDBProductsResponse> Products { get; set; } = [];

    /// <summary>
    /// The list of product countries, retrieved from the database.
    /// </summary>
    public ICollection<GetProductsDbProductCountriesResponse> ProductCountries { get; set; } = [];
}

/// <summary>
/// A product response, retrieved from the database
/// </summary>
public class GetProductsDBProductsResponse
{
    /// <summary>
    /// The Product's Id.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The date and time the product was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The product description.
    /// </summary>
    public string ProductDesc { get; set; } = string.Empty;
}

/// <summary>
/// A product countries response, retrieved from the database
/// </summary>
public class GetProductsDbProductCountriesResponse
{
    /// <summary>
    /// The Product's Id.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The Country's Id.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// The unit price for the country.
    /// </summary>
    public double UnitPrice { get; set; }
}