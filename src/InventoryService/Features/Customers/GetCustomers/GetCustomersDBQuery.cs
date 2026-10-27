using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Queries the database for customers.
/// </summary>
public interface IGetCustomersDBQuery
{
    /// <summary>
    /// Retrieves customers from the database.
    /// </summary>
    /// <param name="dbParams">The database parameters for the query.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A database response containing customers.</returns>
    Task<GetCustomersDBResponse> GetCustomersAsync(GetCustomersDBParameters dbParams, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database for customers.
/// </summary>
public class GetCustomersDBQuery : IGetCustomersDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetCustomersDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    /// <inheritdoc />
    public async Task<GetCustomersDBResponse> GetCustomersAsync(GetCustomersDBParameters dbParams, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "warehouse.usp_GetCustomers",
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