namespace InventoryService.Features.Customers.CreateCustomer;

public interface ICreateCustomerDBParametersMapper
{
    CreateCustomerDBParameters Map(CreateCustomerRequest request);
}

public class CreateCustomerDBParametersMapper : ICreateCustomerDBParametersMapper
{
    public CreateCustomerDBParameters Map(CreateCustomerRequest request)
    {
        return new CreateCustomerDBParameters
        {
            FirstName = request.Payload.FirstName!,
            LastName = request.Payload.LastName!
        };
    }
}
