namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A database response after successfully creating a new customer.
/// </summary>
/// <param name="CustomerId">The Customer's Id.</param>
public record CreateCustomerDBResponse
(
    int CustomerId
);
