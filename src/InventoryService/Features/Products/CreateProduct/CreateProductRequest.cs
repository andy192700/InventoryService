using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace InventoryService.Features.Products.CreateProduct;

public record CreateProductRequest([FromBody]CreateProductRequestPayload Payload)
    : IRequest<Result<CreateProductResponse>>;

public class CreateProductRequestPayload
{
    public string? ProductName { get; set; }

    public string? ProductDesc { get; set; }

    public ICollection<CreateProductRequestProductCountries>? ProductCountries { get; set; }
}

public class CreateProductRequestProductCountries
{
    public string? CountryId { get; set; }

    public string? UnitPrice { get; set; }
}