using Dapper;
using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryService.Features.Customers.CreateCustomer;

public interface ICreateCustomerDBCommand
{
    Task<CreateCustomerDBResponse> CreateCustomerAsync(CreateCustomerDBParameters dbParameters, CancellationToken cancellationToken);
}

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

    public async Task<CreateCustomerDBResponse> CreateCustomerAsync(CreateCustomerDBParameters dbParameters, CancellationToken cancellationToken)
    {

        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var transaction = await conn.BeginTransactionAsync();

        int customerId;

        try
        {
            customerId = await conn.ExecuteScalarAsync<int>(
                "dbo.usp_CreateCustomer",
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
