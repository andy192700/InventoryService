namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A database response containing a product and its associated countries.
/// </summary>
public class GetProductDBResponse
{
    /// <summary>
    /// The product returned by the query.
    /// </summary>
    public GetProductDBProductResponse? Product { get; set; }

    /// <summary>
    /// The countries associated with the product.
    /// </summary>
    public ICollection<GetProductDbProductCountriesResponse> ProductCountries { get; set; } = [];
}

/// <summary>
/// A database response containing a product's details.
/// </summary>
public class GetProductDBProductResponse
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
/// A database response item containing product country details.
/// </summary>
public class GetProductDbProductCountriesResponse
{
    /// <summary>
    /// The Country's Id.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// The unit price for the country.
    /// </summary>
    public double UnitPrice { get; set; }
}