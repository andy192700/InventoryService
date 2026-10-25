namespace InventoryService.Features.Customers.GetCustomers;

public class GetCustomersResponse
{
    public int CustomerId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}
