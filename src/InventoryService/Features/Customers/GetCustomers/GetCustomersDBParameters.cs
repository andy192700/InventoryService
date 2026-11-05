namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Parameters for the database call for listing customers.
/// </summary>
/// <param name="Filter">The search filter.</param>
public record GetCustomersDBParameters
(
    string? Filter
);
