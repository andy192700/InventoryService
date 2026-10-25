namespace InventoryService.Features.Customers.GetCustomer;

public class GetCustomerDBResponse
{
    public int CustomerId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}
