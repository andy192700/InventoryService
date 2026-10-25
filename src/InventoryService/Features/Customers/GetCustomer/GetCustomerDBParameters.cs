namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// Parameters for the database call to retrieve a customer.
/// </summary>
public class GetCustomerDBParameters
{
    /// <summary>
    /// The Customer Id.
    /// </summary>
    public int CustomerId { get; set; }
}
