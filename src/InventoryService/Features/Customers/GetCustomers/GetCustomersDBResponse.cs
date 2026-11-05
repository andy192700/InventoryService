namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// A database response containing customers.
/// </summary>
/// <param name="Customers">The customers returned by the query.</param>
public record GetCustomersDBResponse
(
    ICollection<GetCustomersDBResponseItem> Customers
);

/// <summary>
/// A database response containing a customer's details.
/// </summary>
/// <param name="CustomerId">The Customer's Id.</param>
/// <param name="CreatedDate">The date and time the customer was created.</param>
/// <param name="FirstName">The Customer's first name.</param>
/// <param name="LastName">The Customer's last name.</param>
public record GetCustomersDBResponseItem
(
    int CustomerId,
    DateTimeOffset CreatedDate,
    string FirstName,
    string LastName
);