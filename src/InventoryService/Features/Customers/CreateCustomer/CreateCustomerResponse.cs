namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A response produced after creating a new customer, returned by the <see cref="GetCountriesHandler"/>
/// </summary>
/// <param name="CustomerId">The Customer's Id.</param>
public record CreateCustomerResponse(int CustomerId);
