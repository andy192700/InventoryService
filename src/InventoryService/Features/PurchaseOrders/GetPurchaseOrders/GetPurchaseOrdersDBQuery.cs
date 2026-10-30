using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// Queries the database for purchase orders.
/// </summary>
public interface IGetPurchaseOrdersDBQuery
{
    /// <summary>
    /// Return the purchase orders from the database, as per the supplied parameters.
    /// </summary>
    /// <param name="dbParameters">The database parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task of <see cref="GetPurchaseOrdersDBResponse"/>.</returns>
    Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(GetPurchaseOrdersDBParameters dbParameters, CancellationToken cancellationToken);
}

/// <summary>
/// Queries the database for purchase orders.
/// </summary>
public class GetPurchaseOrdersDBQuery : IGetPurchaseOrdersDBQuery
{
    private readonly ISQLActivity _activity;

    public GetPurchaseOrdersDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<GetPurchaseOrdersDBResponse> GetPurchaseOrdersAsync(
        GetPurchaseOrdersDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_GetPurchaseOrders",
            cancellationToken,
            new
            {
                dBParameters.ProductId,
                dBParameters.CountryId,
                dBParameters.CustomerId
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            var headers = (await result.ReadAsync<GetPurchaseOrdersDBResponseHeader>()).ToList();

            var lines = (await result.ReadAsync<GetPurchaseOrdersDBResponseLine>()).ToList();

            return new GetPurchaseOrdersDBResponse
            {
                Headers = headers,
                Lines = lines
            };
        });
    }
}
