using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Database parameters for creating a product.
/// </summary>
/// <param name="ProductName">The product name.</param>
/// <param name="ProductDesc">The product description.</param>
/// <param name="ProductCountries">The countries associated with the product.</param>
public record CreateProductDBParameters
(
    string ProductName,
    string ProductDesc,
    DataTable ProductCountries
);