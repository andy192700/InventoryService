namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// Parameters for the database call to create a new customer.
/// </summary>
/// <param name="FirstName">The Customer's first name.</param>
/// <param name="LastName">The Customer's last name.</param>
public record CreateCustomerDBParameters
(
    string FirstName,
    string LastName
);
