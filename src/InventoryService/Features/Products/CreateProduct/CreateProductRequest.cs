using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
/// <param name="ProductName">The product name.</param>
/// <param name="ProductDesc">The product description.</param>
/// <param name="ProductCountries">The countries associated with the product.</param>
public record CreateProductRequestPayload(
    string? ProductName,
    string? ProductDesc,
    ICollection<CreateProductRequestProductCountry>? ProductCountries);

/// <summary>
/// A country associated with the product.
/// </summary>
/// <param name="CountryId">The Country's Id.</param>
/// <param name="UnitPrice">The unit price for the country.</param>
public record CreateProductRequestProductCountry(string? CountryId, string? UnitPrice);