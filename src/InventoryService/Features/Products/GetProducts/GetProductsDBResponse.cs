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
/// A database response containing a product's details.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
/// <param name="CreatedDate">The date and time the product was created.</param>
/// <param name="ProductName">The product name.</param>
/// <param name="ProductDesc">The product description.</param>
public record GetProductsDBProductsResponse
(
    int ProductId,
    DateTimeOffset CreatedDate,
    string ProductName,
    string ProductDesc
);

/// <summary>
/// A database response item containing product country details.
/// </summary>
/// <param name="ProductId">The Product Id.</param>
/// <param name="CountryId">The Country's Id.</param>
/// <param name="UnitPrice">The unit price for the country.</param>
public record GetProductsDbProductCountriesResponse
(
    int ProductId,
    int CountryId,
    decimal UnitPrice
);