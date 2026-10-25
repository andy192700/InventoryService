using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

public class CreateProductDBParameters
{
    public required string ProductName { get; set; }


    public required string ProductDesc { get; set; }

    public required DataTable ProductCountries { get; set; }
}
