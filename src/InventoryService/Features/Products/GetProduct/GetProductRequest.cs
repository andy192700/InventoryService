using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.Products.GetProduct;

public record GetProductRequest([FromQuery(Name = "productId")] string? ProductId) : IRequest<Result<GetProductResponse>>;
