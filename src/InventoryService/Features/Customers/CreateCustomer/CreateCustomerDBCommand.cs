using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

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
    private readonly ISQLConnectionProvider _connectionProvider;

    private readonly ILogger<CreateCustomerDBCommand> _logger;

    public CreateCustomerDBCommand(
        ISQLConnectionProvider connectionProvider,
        ILogger<CreateCustomerDBCommand> logger)
    {
        _connectionProvider = connectionProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CreateCustomerDBResponse> CreateCustomerAsync(CreateCustomerDBParameters dbParameters, CancellationToken cancellationToken)
    {

        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var transaction = await conn.BeginTransactionAsync();

        int customerId;

        try
        {
            customerId = await conn.ExecuteScalarAsync<int>(
                "warehouse.usp_CreateCustomer",
                transaction: transaction,
                param: new
                {
                    dbParameters.FirstName,
                    dbParameters.LastName
                },
                commandType: CommandType.StoredProcedure);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                _logger.LogError("Failed to create purchase order");

                await transaction.RollbackAsync();
            }

            throw;
        }

        return new CreateCustomerDBResponse { CustomerId = customerId };
    }
}
