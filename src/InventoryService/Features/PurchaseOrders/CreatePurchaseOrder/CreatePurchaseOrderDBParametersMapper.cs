using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Maps a purchase order request to database parameters.
/// </summary>
public interface ICreatePurchaseOrderDBParametersMapper
{
    /// <summary>
    /// Maps the purchase order request to database parameters.
    /// </summary>
    /// <param name="request">The purchase order request to map.</param>
    /// <returns>Database parameters.</returns>
    CreatePurchaseOrderDBParameters Map(CreatePurchaseOrderRequest request);
}

/// <summary>
/// Maps a purchase order request to database parameters.
/// </summary>
public class CreatePurchaseOrderDBParametersMapper : ICreatePurchaseOrderDBParametersMapper
{
    /// <inheritdoc/>
    public CreatePurchaseOrderDBParameters Map(CreatePurchaseOrderRequest request)
    {
        var linesdt = new DataTable();

        linesdt.Columns.Add("ProductId");
        linesdt.Columns.Add("OrderQuantity");

        foreach (var line in request.Payload.lines!)
        {
            var row = linesdt.NewRow();

            row.SetField("ProductId", int.Parse(line.ProductId!));
            row.SetField("OrderQuantity", int.Parse(line.OrderQuantity!));

            linesdt.Rows.Add(row);
        }

        return new CreatePurchaseOrderDBParameters
        {
            CountryId = int.Parse(request.Payload.CountryId!),
            CustomerId = int.Parse(request.Payload.CustomerId!),
            Lines = linesdt
        };
    }
}
