using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A contract for returning the required infromation after creating a new customer in the database.
/// </summary>
public interface ICreateCustomerDBCommand
{
    /// <summary>
    /// Create the customer in the database.
    /// </summary>
    /// <param name="dbParameters">The database parameters.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task of type <see cref="CreateCustomerDBResponse"/>, containing the database response.</returns>
    Task<CreateCustomerDBResponse> CreateCustomerAsync(CreateCustomerDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Concrete implementation of a <see cref="ICreateCustomerDBCommand"/>, which uses a <see cref="ISQLConnectionProvider"/>
/// to obtain a SQL connection.
/// </summary>
public class CreateCustomerDBCommand : ICreateCustomerDBCommand
{
    private readonly ISQLTransactionActivity _activity;

    private readonly ILogger<CreateCustomerDBCommand> _logger;

    public CreateCustomerDBCommand(
        ISQLTransactionActivity activity,
        ILogger<CreateCustomerDBCommand> logger)
    {
        _activity = activity;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CreateCustomerDBResponse> CreateCustomerAsync(CreateCustomerDBParameters dbParameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_CreateCustomer",
            cancellationToken,
            new
            {
                dbParameters.FirstName,
                dbParameters.LastName
            });

        _activity.WhenCapturingSqlException(sqlEx => _logger.LogError("Failed to create customer."));

        var customerId = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<int>(cmd);
        });

        return new CreateCustomerDBResponse { CustomerId = customerId };
    }
}
