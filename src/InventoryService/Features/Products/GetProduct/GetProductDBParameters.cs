namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// Database parameters for retrieving a product.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
public record GetProductDBParameters(int ProductId);