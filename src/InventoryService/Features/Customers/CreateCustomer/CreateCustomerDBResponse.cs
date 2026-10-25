namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A database response after successfully creating a new customer.
/// </summary>
public class CreateCustomerDBResponse
{
    /// <summary>
    /// The Customer's Id.
    /// </summary>
    public int CustomerId { get; set; }
}
