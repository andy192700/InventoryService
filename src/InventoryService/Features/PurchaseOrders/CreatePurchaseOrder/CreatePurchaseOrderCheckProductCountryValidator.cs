using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;
using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Data validator, validating that all supplied Product-Country mappings exist.
/// </summary>
public class CreatePurchaseOrderCheckProductCountryValidator : DataIntegrityValidator<CreatePurchaseOrderRequest>
{
    /// <inheritdoc/>
    protected override string ErrorMessage => "One or more of the requested Product Countries does not exist.";

    /// <inheritdoc/>
    protected override string StoredProcedureName => "warehouse.usp_CheckPurchaseOrderProductCountry";

    /// <inheritdoc/>
    protected override string[] ApplicableProperties => [nameof(CreatePurchaseOrderRequest.Payload.CountryId), nameof(CreatePurchaseOrderPayloadLine.ProductId)];

    public CreatePurchaseOrderCheckProductCountryValidator(ISQLActivity activity) : base(activity) { }

    /// <inheritdoc/>
    protected override object CreateParameters(CreatePurchaseOrderRequest request)
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

        return new
        {
            request.Payload.CountryId,
            OrderLines = linesdt
        };
    }
}
