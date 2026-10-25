namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A contract for mapping a <see cref="GetCustomerRequest"/> to the required database parameters.
/// </summary>
public interface IGetCustomerDBParametersMapper
{
    /// <summary>
    /// Maps a <see cref="GetCustomerRequest"/> to database parameters for retrieving a customer.
    /// </summary>
    /// <param name="request">The request, derived from the HTTP pipeline.</param>
    /// <returns>The database parameters.</returns>
    GetCustomerDBParameters Map(GetCustomerRequest request);
}

/// <summary>
/// 
/// </summary>
public class GetCustomerDBParametersMapper : IGetCustomerDBParametersMapper
{
    /// <inheritdoc/>
    public GetCustomerDBParameters Map(GetCustomerRequest request)
    {
        return new GetCustomerDBParameters
        {
            CustomerId = int.Parse(request.CustomerId!)
        };
    }
}
