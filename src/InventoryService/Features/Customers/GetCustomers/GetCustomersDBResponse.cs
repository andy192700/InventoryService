namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// A database response containing customers.
/// </summary>
public class GetCustomersDBResponse
{
    /// <summary>
    /// The customers returned by the query.
    /// </summary>
    public required ICollection<GetCustomersDBResponseItem> Customers { get; set; }
}

/// <summary>
/// A database response item containing a customer's details.
/// </summary>
public class GetCustomersDBResponseItem
{
    /// <summary>
    /// The Customer's Id.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The date and time the customer was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The Customer's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The Customer's last name.
    /// </summary>
    public required string LastName { get; set; }
}