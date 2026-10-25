namespace InventoryService.Features.Customers.GetCustomer;

public interface IGetCustomerResponseMapper
{
    GetCustomerResponse Map(GetCustomerDBResponse dBResponse);
}

public class GetCustomerResponseMapper : IGetCustomerResponseMapper
{
    public GetCustomerResponse Map(GetCustomerDBResponse dBResponse)
    {
        return new GetCustomerResponse
        {
            CustomerId = dBResponse.CustomerId,
            CreatedDate = dBResponse.CreatedDate,
            FirstName = dBResponse.FirstName,
            LastName = dBResponse.LastName
        };
    }
}
