namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Database parameters for listing products.
/// </summary>
/// <param name="Filter">The search filter.</param>
public record GetProductsDBParameters
(
    string? Filter
);
