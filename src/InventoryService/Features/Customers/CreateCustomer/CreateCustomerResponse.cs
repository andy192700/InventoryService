namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A response produced after creating a new customer, returned by the <see cref="GetCountriesHandler"/>
/// </summary>
public class CreateCustomerResponse
{
    /// <summary>
    /// 
    /// </summary>
    public int CustomerId { get; set; }
}
