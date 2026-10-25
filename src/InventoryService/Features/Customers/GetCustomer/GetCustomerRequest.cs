using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A request class for retrieving a customer.
/// </summary>
/// <param name="CustomerId">The customer's Id, derived from the request's query.</param>
public record GetCustomerRequest([FromQuery(Name = "customerId")] string? CustomerId) : IRequest<Result<GetCustomerResponse>>;
