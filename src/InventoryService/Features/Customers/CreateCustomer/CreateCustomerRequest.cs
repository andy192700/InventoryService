using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Customers.CreateCustomer;

public record CreateCustomerRequest([FromBody] CreateCustomerRequestPayload Payload) : IRequest<Result<CreateCustomerResponse>>;


public class CreateCustomerRequestPayload
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}