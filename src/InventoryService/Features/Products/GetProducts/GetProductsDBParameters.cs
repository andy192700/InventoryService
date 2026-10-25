namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Database parameters for listing products.
/// </summary>
public class GetProductsDBParameters
{
    /// <summary>
    /// The search filter.
    /// </summary>
    public string? Filter { get; set; }
}
