namespace InventoryService.Features.Customers.GetCustomer;

public interface IGetCustomerDBParametersMapper
{
    GetCustomerDBParameters Map(GetCustomerRequest request);
}

public class GetCustomerDBParametersMapper : IGetCustomerDBParametersMapper
{
    public GetCustomerDBParameters Map(GetCustomerRequest request)
    {
        return new GetCustomerDBParameters
        {
            CustomerId = int.Parse(request.CustomerId!)
        };
    }
}
