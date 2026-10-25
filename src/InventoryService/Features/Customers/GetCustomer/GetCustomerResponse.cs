namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A response containing a customer's details.
/// </summary>
public class GetCustomerResponse
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