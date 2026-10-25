using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// A request to retrieve a list of products.
/// </summary>
/// <param name="Filter">The search filter.</param>
public record GetProductsRequest([FromQuery(Name = "filter")] string? Filter) : IRequest<Result<ICollection<GetProductsResponse>>>;
