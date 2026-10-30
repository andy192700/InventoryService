using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Queries the database for a list of products.
/// </summary>
public interface IGetProductsDBQuery
{
    /// <summary>
    /// Retrieves a list of products from the database.
    /// </summary>
    /// <param name="dbParameters">The database parameters for the query.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A database response containing the list of products.</returns>
    Task<GetProductsDBResponse> GetProductsAsync(GetProductsDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database for a list of products.
/// </summary>
public class GetProductsDBQuery : IGetProductsDBQuery
{
    private readonly ISQLActivity _activity;

    public GetProductsDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<GetProductsDBResponse> GetProductsAsync(GetProductsDBParameters dbParameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetProducts",
            cancellationToken,
            new
            {
                dbParameters.Filter
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            var products = (await result.ReadAsync<GetProductsDBProductsResponse>()).ToList();

            var productCountries = (await result.ReadAsync<GetProductsDbProductCountriesResponse>()).ToList();

            return new GetProductsDBResponse
            {
                Products = products,
                ProductCountries = productCountries
            };
        });
    }
}
