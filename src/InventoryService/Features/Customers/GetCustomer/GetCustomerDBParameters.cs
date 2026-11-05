namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// Parameters for the database call to retrieve a customer.
/// </summary>
/// <param name="CustomerId">The Customer Id.</param>
public record GetCustomerDBParameters
(
    int CustomerId
);
