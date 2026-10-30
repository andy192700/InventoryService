using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A contract for returning a customer from the database.
/// </summary>
public interface IGetCustomerDBQuery
{
    /// <summary>
    /// Retrieves a customer from the database.
    /// </summary>
    /// <param name="dBParameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The database representation of a customer.</returns>
    Task<GetCustomerDBResponse?> GetCustomerAsync(GetCustomerDBParameters dBParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Concrete implementation of a <see cref="IGetCustomerDBQuery"/>, which uses a <see cref="ISQLConnectionProvider"/>
/// to obtain a SQL connection.
/// </summary>
public class GetCustomerDBQuery : IGetCustomerDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetCustomerDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    /// <inheritdoc/>
    public async Task<GetCustomerDBResponse?> GetCustomerAsync(GetCustomerDBParameters dBParameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        var cmd = new CommandDefinition(
            "warehouse.usp_GetCustomer",
            parameters: new
            {
                dBParameters.CustomerId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var result = await conn.QueryMultipleAsync(cmd);

        return await result.ReadFirstOrDefaultAsync<GetCustomerDBResponse>();
    }
}
