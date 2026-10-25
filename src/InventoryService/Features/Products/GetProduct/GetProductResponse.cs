namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A response containing a product's details.
/// </summary>
public class GetProductResponse
{
    /// <summary>
    /// The Product's Id.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The date and time the customer was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The Product's Name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The Product's Description.
    /// </summary>
    public string ProductDesc { get; set; } = string.Empty;

    /// <summary>
    /// The Product Countries.
    /// </summary>
    public ICollection<GetProductCountriesResponse> ProductCountries { get; set; } = [];
}

/// <summary>
/// Represents a Product's price per Country of origin.
/// </summary>
public class GetProductCountriesResponse
{
    /// <summary>
    /// The Country Id.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// The Unit Price of the parent product in the specified country.
    /// </summary>
    public double UnitPrice { get; set; }
}
