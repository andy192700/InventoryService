namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A response after successfully creating a new product.
/// </summary>
public class CreateProductResponse
{
    /// <summary>
    /// The Product's Id.
    /// </summary>
    public int ProductId { get; set; }
}