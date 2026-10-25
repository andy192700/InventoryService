using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Products.GetProducts;

public record GetProductsRequest([FromQuery(Name = "filter")] string? Filter) : IRequest<Result<ICollection<GetProductsResponse>>>;
