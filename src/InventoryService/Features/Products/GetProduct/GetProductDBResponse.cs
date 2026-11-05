namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A database response containing a product and its associated countries.
/// </summary>
/// <param name="Product">The product returned by the query.</param>
/// <param name="ProductCountries">The countries associated with the product.</param>
public record GetProductDBResponse
(
    GetProductDBProductResponse? Product,
    ICollection<GetProductDbProductCountriesResponse> ProductCountries
);

/// <summary>
/// A database response containing a product's details.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
/// <param name="CreatedDate">The date and time the product was created.</param>
/// <param name="ProductName">The product name.</param>
/// <param name="ProductDesc">The product description.</param>
public record GetProductDBProductResponse
(
    int ProductId,
    DateTimeOffset CreatedDate,
    string ProductName,
    string ProductDesc
);

/// <summary>
/// A database response item containing product country details.
/// </summary>
/// <param name="CountryId">The Country's Id.</param>
/// <param name="UnitPrice">The unit price for the country.</param>
public record GetProductDbProductCountriesResponse
(
    int CountryId,
    decimal UnitPrice
);