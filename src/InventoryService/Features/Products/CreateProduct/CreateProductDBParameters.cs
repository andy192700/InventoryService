using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Database parameters for creating a product.
/// </summary>
public class CreateProductDBParameters
{
    /// <summary>
    /// The product name.
    /// </summary>
    public required string ProductName { get; set; }

    /// <summary>
    /// The product description.
    /// </summary>
    public required string ProductDesc { get; set; }

    /// <summary>
    /// The countries associated with the product.
    /// </summary>
    public required DataTable ProductCountries { get; set; }
}