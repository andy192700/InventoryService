namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A response after successfully creating a new product.
/// </summary>
/// <param name="ProductId">The Product's Id.</param>
public record CreateProductResponse
(
    int ProductId
);