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
public class CreateCustomerRequestPayload
{
    /// <summary>
    /// The Customer's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The Customer's last name.
    /// </summary>
    public string? LastName { get; set; }
}