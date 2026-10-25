namespace InventoryService.Features.Products.GetProducts;

public interface IGetProductsDBParametersMapper
{
    GetProductsDBParameters Map(GetProductsRequest request);
}

public class GetProductsDBParametersMapper : IGetProductsDBParametersMapper
{
    public GetProductsDBParameters Map(GetProductsRequest request)
    {
        return new GetProductsDBParameters
        {
            Filter = request.Filter
        };
    }
}