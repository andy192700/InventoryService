using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A request to retrieve a product.
/// </summary>
/// <param name="ProductId">The Product Id.</param>
public record GetProductRequest([FromQuery(Name = "productId")] string? ProductId) : IRequest<Result<GetProductResponse>>;
