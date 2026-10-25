namespace InventoryService.Features.Products.GetProduct;

public interface IGetProductDBParametersMapper
{
    GetProductDBParameters Map(GetProductRequest getProductRequest);
}

public class GetProductDBParametersMapper : IGetProductDBParametersMapper
{
    public GetProductDBParameters Map(GetProductRequest getProductRequest)
    {
        return new(int.Parse(getProductRequest.ProductId!));
    }
}
