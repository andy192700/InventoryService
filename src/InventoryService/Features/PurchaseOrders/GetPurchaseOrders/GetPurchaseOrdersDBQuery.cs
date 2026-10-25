using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public interface IGetPurchaseOrdersDBQuery
{
    Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(GetPurchaseOrdersDBParameters dbParameters, CancellationToken cancellationToken);
}

public class GetPurchaseOrdersDBQuery : IGetPurchaseOrdersDBQuery
{
    private readonly ISQLConnectionProvider _connectionProvider;

    public GetPurchaseOrdersDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(
        GetPurchaseOrdersDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetPurchaseOrders",
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
