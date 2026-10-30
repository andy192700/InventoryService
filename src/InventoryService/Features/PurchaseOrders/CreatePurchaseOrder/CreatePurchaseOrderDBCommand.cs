using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

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
    private readonly ISQLTransactionActivity _activity;

    private readonly ILogger<CreatePurchaseOrderDBCommand> _logger;

    public CreatePurchaseOrderDBCommand(
        ISQLTransactionActivity activity,
        ILogger<CreatePurchaseOrderDBCommand> logger)
    {
        _activity = activity;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CreatePurchaseOrderDBResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderDBParameters parameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_CreatePurchaseOrder", 
            cancellationToken,
            new
            {
                parameters.CustomerId,
                parameters.CountryId,
                OrderLines = parameters.Lines
            });

        _activity.WhenCapturingSqlException(sqlEx => _logger.LogError("Failed to create purchase order"));

        var purchaseOrderId = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<int>(cmd);
        });

        return new CreatePurchaseOrderDBResponse
        {
            PurchaseOrderId = purchaseOrderId,
        };
    }
}
