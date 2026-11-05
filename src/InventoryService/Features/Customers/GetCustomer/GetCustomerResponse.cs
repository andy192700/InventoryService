namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A response containing a customer's details.
/// </summary>
/// <param name="CustomerId">The Customer's Id.</param>
/// <param name="CreatedDate">The date and time the customer was created.</param>
/// <param name="FirstName">The Customer's first name.</param>
/// <param name="LastName">The Customer's last name.</param>
public record GetCustomerResponse
(
    int CustomerId,
    DateTimeOffset CreatedDate,
    string FirstName,
    string LastName
);