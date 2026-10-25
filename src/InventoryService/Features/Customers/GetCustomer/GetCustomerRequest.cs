using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.GetCustomer;

public record GetCustomerRequest([FromQuery(Name = "customerId")] string? CustomerId) : IRequest<Result<GetCustomerResponse>>;
