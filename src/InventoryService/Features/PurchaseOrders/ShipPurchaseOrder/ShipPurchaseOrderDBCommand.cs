using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;

public interface IShipPurchaseOrderDBCommand
{
    Task<bool> MarkAsShippedAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken);
}

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
