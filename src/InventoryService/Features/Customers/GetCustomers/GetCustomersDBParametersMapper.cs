namespace InventoryService.Features.Customers.GetCustomers;

public interface IGetCustomersDBParametersMapper
{
    GetCustomersDBParameters Map(GetCustomersRequest request);
}

public class GetCustomersDBParametersMapper : IGetCustomersDBParametersMapper
{
    public GetCustomersDBParameters Map(GetCustomersRequest request)
    {
        return new GetCustomersDBParameters
        {
            Filter = request.Filter
        };
    }
}
