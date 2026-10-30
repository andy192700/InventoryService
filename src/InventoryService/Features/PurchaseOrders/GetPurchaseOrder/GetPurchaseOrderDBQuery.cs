using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// Queries the database to obtain a purchase order.
/// </summary>
public interface IGetPurchaseOrderDBQuery
{
    /// <summary>
    /// Query the database for a purchase order.
    /// </summary>
    /// <param name="dbParameters">The database parameters.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The database purchase order representation.</returns>
    Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database to obtain a purchase order.
/// </summary>
public class GetPurchaseOrderDBQuery : IGetPurchaseOrderDBQuery
{
    private readonly ISQLActivity _activity;

    public GetPurchaseOrderDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<GetPurchaseOrderDBResponse> GetPurchaseOrderAsync(GetPurchaseOrderDBParameters dbParameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetPurchaseOrder",
            cancellationToken,
            new
            {
                dbParameters.PurchaseOrderId
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            var header = await result.ReadFirstOrDefaultAsync<GetPurchaseOrderDBResponseHeader>();

            var lines = (await result.ReadAsync<GetPurchaseOrderDBResponseLine>()).ToList();

            return new GetPurchaseOrderDBResponse
            {
                header = header,
                lines = lines
            };
        });
    }
}
