using Dapper;
using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Creates a new Purchesse Order in the database.
/// </summary>
public interface ICreatePurchaseOrderDBCommand
{
    /// <summary>
    /// Creates a purchase order in the database asynchronously.
    /// </summary>
    /// <param name="parameters">The parameters required to create the purchase order including customer, country, and order lines.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="CreatePurchaseOrderDBResponse"/> containing the ID of the newly created purchase order.</returns>
    Task<CreatePurchaseOrderDBResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderDBParameters parameters, CancellationToken cancellationToken);
}

/// <summary>
/// Creates a new Purchesse Order in the database.
/// </summary>
public class CreatePurchaseOrderDBCommand : ICreatePurchaseOrderDBCommand
{
    private readonly ISQLConnectionProvider _connectionProvider;

    private readonly ILogger<CreatePurchaseOrderDBCommand> _logger;

    public CreatePurchaseOrderDBCommand(
        ISQLConnectionProvider connectionProvider,
        ILogger<CreatePurchaseOrderDBCommand> logger)
    {
        _connectionProvider = connectionProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CreatePurchaseOrderDBResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderDBParameters parameters, CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var transaction = await conn.BeginTransactionAsync();

        int purchaseOrderId;

        try
        {
            purchaseOrderId = await conn.ExecuteScalarAsync<int>(
                "dbo.usp_CreatePurchaseOrder",
                transaction: transaction,
                param: new
                {
                    parameters.CustomerId,
                    parameters.CountryId,
                    OrderLines = parameters.Lines
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

        return new CreatePurchaseOrderDBResponse
        {
            PurchaseOrderId = purchaseOrderId,
        };
    }
}
