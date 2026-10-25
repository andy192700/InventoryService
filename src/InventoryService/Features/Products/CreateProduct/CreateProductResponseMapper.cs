namespace InventoryService.Features.Products.CreateProduct;

public interface ICreateProductResponseMapper
{
    CreateProductResponse Map(CreateProductDBResponse dBResponse);
}

public class CreateProductResponseMapper : ICreateProductResponseMapper
{
    public CreateProductResponse Map(CreateProductDBResponse dBResponse)
    {
        return new CreateProductResponse
        {
            ProductId = dBResponse.ProductId
        };
    }
}
