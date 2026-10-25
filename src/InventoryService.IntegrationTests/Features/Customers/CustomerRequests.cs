namespace InventoryService.IntegrationTests.Features.Customers;

public class CustomerRequests
{
    public class CreateCustomerIntRequest
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
