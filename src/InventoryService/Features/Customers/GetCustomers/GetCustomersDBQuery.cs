using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

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
    private readonly ISQLActivity _activity;

    public GetCustomersDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc />
    public async Task<GetCustomersDBResponse> GetCustomersAsync(GetCustomersDBParameters dbParams, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetCustomers",
            cancellationToken,
            new
            {
                dbParams.Filter
            });

        var customers = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            return (await result.ReadAsync<GetCustomersDBResponseItem>()).ToList();
        });

        return new GetCustomersDBResponse
        {
            Customers = customers
        };
    }
}