namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A response containing a product's details.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
/// <param name="CreatedDate">The date and time the customer was created.</param>
/// <param name="ProductName">The Product's Name.</param>
/// <param name="ProductDesc">The Product's Description.</param>
/// <param name="ProductCountries">The Product Countries.</param>
public record GetProductResponse
(
    int ProductId,
    DateTimeOffset CreatedDate,
    string ProductName,
    string ProductDesc,
    ICollection<GetProductCountriesResponse> ProductCountries
);

/// <summary>
/// Contains a Product's per country pricing.
/// </summary>
/// <param name="CountryId">The Country Id.</param>
/// <param name="UnitPrice">The Unit Price of the parent product in the specified country.</param>
public record GetProductCountriesResponse
(
    int CountryId,
    double UnitPrice
);