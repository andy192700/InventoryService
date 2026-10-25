namespace InventoryService.Features.Customers.GetCustomers;

public class GetCustomersDBResponse
{
    public required ICollection<GetCustomersDBResponseItem> Customers { get; set; }
}

public class GetCustomersDBResponseItem
{
    public int CustomerId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}