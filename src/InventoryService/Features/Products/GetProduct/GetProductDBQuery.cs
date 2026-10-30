using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// Queries the database for a product.
/// </summary>
public interface IGetProductDBQuery
{
    /// <summary>
    /// Retrieves a product from the database.
    /// </summary>
    /// <param name="dBParameters">The database parameters for the query.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A database response containing the product.</returns>
    Task<GetProductDBResponse> GetProductAsync(
        GetProductDBParameters dBParameters,
        CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database for a product.
/// </summary>
public class GetProductDBQuery : IGetProductDBQuery
{
    private readonly ISQLActivity _activity;

    public GetProductDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc />
    public async Task<GetProductDBResponse> GetProductAsync(
        GetProductDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetProduct",
            cancellationToken,
            new
            {
                dBParameters.ProductId
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            var product = await result.ReadFirstOrDefaultAsync<GetProductDBProductResponse>();

            var productCountries = (await result.ReadAsync<GetProductDbProductCountriesResponse>()).ToList();

            return new GetProductDBResponse
            {
                Product = product,
                ProductCountries = productCountries
            };
        });
    }
}