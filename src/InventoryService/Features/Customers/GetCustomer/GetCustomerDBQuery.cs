using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Customers.GetCustomer;

public interface IGetCustomerDBQuery
{
    Task<GetCustomerDBResponse?> GetCustomerAsync(GetCustomerDBParameters dBParameters, CancellationToken cancellationToken);
}

public class GetCustomerDBQuery : IGetCustomerDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetCustomerDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    public async Task<GetCustomerDBResponse?> GetCustomerAsync(GetCustomerDBParameters dBParameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetCustomer",
            param: new
            {
                dBParameters.CustomerId
            },
            commandType: CommandType.StoredProcedure);

        return await result.ReadFirstOrDefaultAsync<GetCustomerDBResponse>();
    }
}
