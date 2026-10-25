namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A contract for creating create customer database parameters from a request.
/// </summary>
public interface ICreateCustomerDBParametersMapper
{
    /// <summary>
    /// Maps a <see cref="CreateCustomerRequest"/> to database parameters for creating a customer.
    /// </summary>
    /// <param name="request">The request, derived from the HTTP pipeline.</param>
    /// <returns>The database parameters.</returns>
    CreateCustomerDBParameters Map(CreateCustomerRequest request);
}

/// <summary>
/// Concrete implementation for a <see cref="ICreateCustomerDBParametersMapper"/>
/// </summary>
public class CreateCustomerDBParametersMapper : ICreateCustomerDBParametersMapper
{
    /// <inheritdoc/>
    public CreateCustomerDBParameters Map(CreateCustomerRequest request)
    {
        return new CreateCustomerDBParameters
        {
            FirstName = request.Payload.FirstName!,
            LastName = request.Payload.LastName!
        };
    }
}
