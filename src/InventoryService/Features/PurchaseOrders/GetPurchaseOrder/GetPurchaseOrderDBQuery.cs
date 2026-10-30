using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// Queries the database to obtain a purchase order.
/// </summary>
public interface IGetPurchaseOrderDBQuery
{
    /// <summary>
    /// Query the database for a purchase order.
    /// </summary>
    /// <param name="dbParameters">The database parameters.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The database purchase order representation.</returns>
    Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database to obtain a purchase order.
/// </summary>
public class GetPurchaseOrderDBQuery : IGetPurchaseOrderDBQuery
{
    private readonly ISQLConnectionProvider _connectionProvider;

    public GetPurchaseOrderDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    /// <inheritdoc/>
    public async Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        var cmd = new CommandDefinition(
            "warehouse.usp_GetPurchaseOrder",
            parameters: new
            {
                dbParameters.PurchaseOrderId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var result = await conn.QueryMultipleAsync(cmd);

        var header = await result.ReadFirstOrDefaultAsync<GetPurchaseOrderDBResponseHeader>();

        var lines = (await result.ReadAsync<GetPurchaseOrderDBResponseLine>()).ToList();

        return new GetPurchaseOrderDBResponse
        {
            header = header,
            lines = lines
        };
    }
}
