namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A contract for mapping a create customer database response to a <see cref="CreateCustomerResponse"/>.
/// </summary>
public interface ICreateCustomerResponseMapper
{
    /// <summary>
    /// Map the create customer database response to a create country response.
    /// </summary>
    /// <param name="dbResponse">The database response.</param>
    /// <returns>The create customer response.</returns>
    CreateCustomerResponse Map(CreateCustomerDBResponse dbResponse);
}

/// <summary>
/// Concrete implementation for a <see cref="ICreateCustomerResponseMapper"/>.
/// </summary>
public class CreateCustomerResponseMapper : ICreateCustomerResponseMapper
{
    /// <inheritdoc/>
    public CreateCustomerResponse Map(CreateCustomerDBResponse dbResponse)
    {
        return new CreateCustomerResponse
        {
            CustomerId = dbResponse.CustomerId
        };
    }
}
