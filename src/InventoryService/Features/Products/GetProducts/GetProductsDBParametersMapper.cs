namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Maps a products request to database parameters.
/// </summary>
public interface IGetProductsDBParametersMapper
{
    /// <summary>
    /// Maps the products request to database parameters.
    /// </summary>
    /// <param name="request">The products request to map.</param>
    /// <returns>Database parameters.</returns>
    GetProductsDBParameters Map(GetProductsRequest request);
}

/// <summary>
/// Maps a products request to database parameters.
/// </summary>
public class GetProductsDBParametersMapper : IGetProductsDBParametersMapper
{
    /// <inheritdoc/>
    public GetProductsDBParameters Map(GetProductsRequest request)
    {
        return new GetProductsDBParameters
        {
            Filter = request.Filter
        };
    }
}