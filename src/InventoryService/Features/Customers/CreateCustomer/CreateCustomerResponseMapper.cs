namespace InventoryService.Features.Customers.CreateCustomer;

public interface ICreateCustomerResponseMapper
{
    CreateCustomerResponse Map(CreateCustomerDBResponse dbResponse);
}

public class CreateCustomerResponseMapper : ICreateCustomerResponseMapper
{
    public CreateCustomerResponse Map(CreateCustomerDBResponse dbResponse)
    {
        return new CreateCustomerResponse
        {
            CustomerId = dbResponse.CustomerId
        };
    }
}
