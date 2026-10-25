namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Maps a customer request to database parameters.
/// </summary>
public interface IGetCustomersDBParametersMapper
{
    /// <summary>
    /// Maps the customer request to database parameters.
    /// </summary>
    /// <param name="request">The customer request to map.</param>
    /// <returns>Database parameters.</returns>
    GetCustomersDBParameters Map(GetCustomersRequest request);
}

/// <summary>
/// Maps a customer request to database parameters.
/// </summary>
public class GetCustomersDBParametersMapper : IGetCustomersDBParametersMapper
{
    /// <inheritdoc/>
    public GetCustomersDBParameters Map(GetCustomersRequest request)
    {
        return new GetCustomersDBParameters
        {
            Filter = request.Filter
        };
    }
}
