using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A request class for retrieving countries.
/// </summary>
/// <param name="Payload">The payload of the request, derived from the request body.</param>
public record CreateCustomerRequest([FromBody] CreateCustomerRequestPayload Payload) : IRequest<Result<CreateCustomerResponse>>;

/// <summary>
/// The request payload for retrieving Countries.
/// </summary>
/// <param name="FirstName">The Customer's first name.</param>
/// <param name="LastName">The Customer's last name.</param>
public record CreateCustomerRequestPayload(string? FirstName, string? LastName);