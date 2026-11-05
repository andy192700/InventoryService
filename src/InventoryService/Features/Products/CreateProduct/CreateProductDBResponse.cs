namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A database response after successfully creating a new product.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
public record CreateProductDBResponse
(
    int ProductId
);