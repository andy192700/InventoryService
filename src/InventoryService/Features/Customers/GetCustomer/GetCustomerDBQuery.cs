using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

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
    private readonly ISQLActivity _activity;

    public GetCustomerDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<GetCustomerDBResponse?> GetCustomerAsync(GetCustomerDBParameters dBParameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetCustomer",
            cancellationToken,
            new
            {
                dBParameters.CustomerId
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.QueryFirstOrDefaultAsync<GetCustomerDBResponse?>(cmd);
        });
    }
}
