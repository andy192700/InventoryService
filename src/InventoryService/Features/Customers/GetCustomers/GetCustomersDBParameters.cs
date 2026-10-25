namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Parameters for the database call for listing customers.
/// </summary>
public class GetCustomersDBParameters
{
    /// <summary>
    /// the search filter.
    /// </summary>
    public string? Filter { get; set; }
}
