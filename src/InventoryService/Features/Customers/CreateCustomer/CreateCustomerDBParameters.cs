namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// Parameters for the database call to create a new customer.
/// </summary>
public class CreateCustomerDBParameters
{
    /// <summary>
    /// The Customer's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The Customer's last name.
    /// </summary>
    public required string LastName { get; set; }
}
