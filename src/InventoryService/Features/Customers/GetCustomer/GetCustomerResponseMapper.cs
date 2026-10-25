namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// Maps a database response to a customer response.
/// </summary>
public interface IGetCustomerResponseMapper
{
    /// <summary>
    /// Maps the database response to a customer response.
    /// </summary>
    /// <param name="dBResponse">The database response to map.</param>
    /// <returns>A customer response.</returns>
    GetCustomerResponse Map(GetCustomerDBResponse dBResponse);
}

/// <summary>
/// Maps a database response to a customer response.
/// </summary>
public class GetCustomerResponseMapper : IGetCustomerResponseMapper
{
    /// <inheritdoc/>
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