using Dapper;
using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

public interface ICreatePurchaseOrderDBCommand
{
    Task<CreatePurchaseOrderDBResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderDBParameters parameters, CancellationToken cancellationToken);
}

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
