namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Maps a database response to customer responses.
/// </summary>
public interface IGetCustomersResponseMapper
{
    /// <summary>
    /// Maps the database response to customer responses.
    /// </summary>
    /// <param name="dBResponse">The database response to map.</param>
    /// <returns>Customer responses.</returns>
    ICollection<GetCustomersResponse> Map(GetCustomersDBResponse dBResponse);
}

/// <summary>
/// Maps a database response to customer responses.
/// </summary>
public class GetCustomersResponseMapper : IGetCustomersResponseMapper
{
    /// <inheritdoc />
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