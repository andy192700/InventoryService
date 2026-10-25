namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Maps a database response to a product response.
/// </summary>
public interface ICreateProductResponseMapper
{
    /// <summary>
    /// Maps the database response to a product response.
    /// </summary>
    /// <param name="dBResponse">The database response to map.</param>
    /// <returns>A product response.</returns>
    CreateProductResponse Map(CreateProductDBResponse dBResponse);
}

/// <summary>
/// Maps a database response to a product response.
/// </summary>
public class CreateProductResponseMapper : ICreateProductResponseMapper
{
    /// <inheritdoc />
    public CreateProductResponse Map(CreateProductDBResponse dBResponse)
    {
        return new CreateProductResponse
        {
            ProductId = dBResponse.ProductId
        };
    }
}