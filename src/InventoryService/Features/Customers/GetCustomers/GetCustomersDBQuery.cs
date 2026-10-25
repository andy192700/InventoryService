using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Customers.GetCustomers;

public interface IGetCustomersDBQuery
{
    Task<GetCustomersDBResponse> GetCustomersAsync(GetCustomersDBParameters dbParams, CancellationToken cancellationToken);
}

public class GetCustomersDBQuery : IGetCustomersDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetCustomersDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    public async Task<GetCustomersDBResponse> GetCustomersAsync(GetCustomersDBParameters dbParams, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetCustomers",
            param: new
            {
                dbParams.Filter
            },
            commandType: CommandType.StoredProcedure);

        return new GetCustomersDBResponse
        {
            Customers = (await result.ReadAsync<GetCustomersDBResponseItem>()).ToList()
        };
    }
}
