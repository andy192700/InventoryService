namespace InventoryService.Features.Customers.GetCustomers;

public interface IGetCustomersResponseMapper
{
    ICollection<GetCustomersResponse> Map(GetCustomersDBResponse dBResponse);
}

public class GetCustomersResponseMapper : IGetCustomersResponseMapper
{
    public ICollection<GetCustomersResponse> Map(GetCustomersDBResponse dBResponse)
    {
        var customers = new List<GetCustomersResponse>();

        foreach (var customer in dBResponse.Customers)
        {
            customers.Add(new GetCustomersResponse
            {
                CustomerId = customer.CustomerId,
                CreatedDate = customer.CreatedDate,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            });
        }

        return customers;
    }
}
