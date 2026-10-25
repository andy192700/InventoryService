using System.Data;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

public interface ICreatePurchaseOrderDBParametersMapper
{
    CreatePurchaseOrderDBParameters Map(CreatePurchaseOrderRequest request);
}

public class CreatePurchaseOrderDBParametersMapper : ICreatePurchaseOrderDBParametersMapper
{
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
