namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// Maps a product request to database parameters.
/// </summary>
public interface IGetProductDBParametersMapper
{
    /// <summary>
    /// Maps the product request to database parameters.
    /// </summary>
    /// <param name="getProductRequest">The product request to map.</param>
    /// <returns>Database parameters.</returns>
    GetProductDBParameters Map(GetProductRequest getProductRequest);
}

/// <summary>
/// Maps a product request to database parameters.
/// </summary>
public class GetProductDBParametersMapper : IGetProductDBParametersMapper
{
    /// <inheritdoc />
    public GetProductDBParameters Map(GetProductRequest getProductRequest)
    {
        return new(int.Parse(getProductRequest.ProductId!));
    }
}