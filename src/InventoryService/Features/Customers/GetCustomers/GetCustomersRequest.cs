using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.GetCustomers;

public record GetCustomersRequest([FromQuery(Name = "filter")] string? Filter) : IRequest<Result<ICollection<GetCustomersResponse>>>;
