using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public interface IGetPurchaseOrderDBQuery
{
    Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken);
}

public class GetPurchaseOrderDBQuery : IGetPurchaseOrderDBQuery
{
    private readonly ISQLConnectionProvider _connectionProvider;

    public GetPurchaseOrderDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetPurchaseOrder",
            param: new
            {
                dbParameters.PurchaseOrderId
            },
            commandType: CommandType.StoredProcedure);

        var header = await result.ReadFirstOrDefaultAsync<GetPurchaseOrderDBResponseHeader>();

        var lines = (await result.ReadAsync<GetPurchaseOrderDBResponseLine>()).ToList();

        return new GetPurchaseOrderDBResponse
        {
            header = header,
            lines = lines
        };
    }
}
