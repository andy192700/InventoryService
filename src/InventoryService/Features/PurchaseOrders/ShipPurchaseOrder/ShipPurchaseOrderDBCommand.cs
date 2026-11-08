using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;

/// <summary>
/// Command to mark a Purchase Order as shipped.
/// </summary>
public interface IShipPurchaseOrderDBCommand
{
    /// <summary>
    /// Marks the supplied order as shipped.
    /// </summary>
    /// <param name="purchaseOrderId">The purchase order id.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A bool indicating whether the purchase order status was updated as expected.</returns>
    Task<bool> MarkAsShippedAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Command to mark a Purchase Order as shipped.
/// </summary>
public class ShipPurchaseOrderDBCommand : IShipPurchaseOrderDBCommand
{
    private readonly ISQLTransactionActivity _activity;

    private readonly ILogger<ShipPurchaseOrderDBCommand> _logger;

    public ShipPurchaseOrderDBCommand(
        ISQLTransactionActivity activity,
        ILogger<ShipPurchaseOrderDBCommand> logger)
    {
        _activity = activity;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkAsShippedAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_MarkPurchaseOrderAsShipped",
            cancellationToken,
            new
            {
                PurchaseOrderId = purchaseOrderId
            });

        _activity.WhenCapturingSqlException(sqlEx => _logger.LogError("Failed to create purchase order"));

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<bool>(cmd);
        });
    }
}
