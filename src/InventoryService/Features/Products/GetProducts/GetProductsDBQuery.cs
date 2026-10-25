using Dapper;
using InventoryService.Infrastructure;
using System.Data;

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
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetProductsDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _sqlConnectionProvider = connectionProvider;
    }

    /// <inheritdoc/>
    public async Task<GetProductsDBResponse> GetProductsAsync(GetProductsDBParameters dbParameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetProducts",
            param: new
            {
                dbParameters.Filter
            },
            commandType: CommandType.StoredProcedure);

        var products = (await result.ReadAsync<GetProductsDBProductsResponse>()).ToList(); 

        var productCountries = (await result.ReadAsync<GetProductsDbProductCountriesResponse>()).ToList();

        return new GetProductsDBResponse
        {
            Products = products,
            ProductCountries = productCountries
        };
    }
}
