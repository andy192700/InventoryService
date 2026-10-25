namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A database response after successfully creating a new product.
/// </summary>
public class CreateProductDBResponse
{
    /// <summary>
    /// The Product's Id.
    /// </summary>
    public int ProductId { get; set; }
}