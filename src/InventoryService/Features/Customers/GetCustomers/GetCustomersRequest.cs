using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// A request class for retrieving a list of customers.
/// </summary>
/// <param name="Filter">The search filter.</param>
public record GetCustomersRequest([FromQuery(Name = "filter")] string? Filter) : IRequest<Result<ICollection<GetCustomersResponse>>>;
