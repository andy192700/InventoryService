using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A request to create a product.
/// </summary>
/// <param name="Payload">The request payload.</param>
public record CreateProductRequest([FromBody] CreateProductRequestPayload Payload)
    : IRequest<Result<CreateProductResponse>>;

/// <summary>
/// The payload for a create product request.
/// </summary>
public class CreateProductRequestPayload
{
    /// <summary>
    /// The product name.
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// The product description.
    /// </summary>
    public string? ProductDesc { get; set; }

    /// <summary>
    /// The countries associated with the product.
    /// </summary>
    public ICollection<CreateProductRequestProductCountries>? ProductCountries { get; set; }
}

/// <summary>
/// A country associated with the product.
/// </summary>
public class CreateProductRequestProductCountries
{
    /// <summary>
    /// The Country's Id.
    /// </summary>
    public string? CountryId { get; set; }

    /// <summary>
    /// The unit price for the country.
    /// </summary>
    public string? UnitPrice { get; set; }
}