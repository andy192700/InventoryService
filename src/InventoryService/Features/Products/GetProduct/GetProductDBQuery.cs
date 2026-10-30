using Dapper;
using InventoryService.Infrastructure;
using System.Data;

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
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetProductDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    /// <inheritdoc />
    public async Task<GetProductDBResponse> GetProductAsync(
        GetProductDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        var cmd = new CommandDefinition(
            "warehouse.usp_GetProduct",
            parameters: new
            {
                dBParameters.ProductId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var result = await conn.QueryMultipleAsync(cmd);

        var product = await result.ReadFirstOrDefaultAsync<GetProductDBProductResponse>();

        var productCountries = (await result.ReadAsync<GetProductDbProductCountriesResponse>()).ToList();

        return new GetProductDBResponse
        {
            Product = product,
            ProductCountries = productCountries
        };
    }
}