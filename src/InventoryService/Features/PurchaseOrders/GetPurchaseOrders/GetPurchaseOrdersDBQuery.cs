using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// Queries the database for purchase orders.
/// </summary>
public interface IGetPurchaseOrdersDBQuery
{
    /// <summary>
    /// Return the purchase orders from the database, as per the supplied parameters.
    /// </summary>
    /// <param name="dbParameters">The database parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task of <see cref="GetPurchaseOrdersDBResponse"/>.</returns>
    Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(GetPurchaseOrdersDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database for purchase orders.
/// </summary>
public class GetPurchaseOrdersDBQuery : IGetPurchaseOrdersDBQuery
{
    private readonly ISQLConnectionProvider _connectionProvider;

    public GetPurchaseOrdersDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    /// <inheritdoc/>
    public async Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(
        GetPurchaseOrdersDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "warehouse.usp_GetPurchaseOrders",
            param: new
            {
                dBParameters.ProductId,
                dBParameters.CountryId,
                dBParameters.CustomerId
            },
            commandType: CommandType.StoredProcedure);

        var headers = (await result.ReadAsync<GetPurchaseOrdersDBResponseHeader>()).ToList();

        var lines = (await result.ReadAsync<GetPurchaseOrdersDBResponseLine>()).ToList();

        return new GetPurchaseOrdersDBResponse
        {
            Headers = headers,
            Lines = lines
        };
    }
}
