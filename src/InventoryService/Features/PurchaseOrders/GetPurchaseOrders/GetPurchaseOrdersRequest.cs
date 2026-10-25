using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A purchase order's request
/// </summary>
/// <param name="ProductId">The product Id.</param>
/// <param name="CountryId">The Country id.</param>
/// <param name="CustomerId">The Customer id.</param>
public record GetPurchaseOrdersRequest(
    [FromQuery(Name = "productId")] string? ProductId,
    [FromQuery(Name = "countryId")] string? CountryId,
    [FromQuery(Name = "customerId")] string? CustomerId
    ) : IRequest<Result<ICollection<GetPurchaseOrdersResponse>>>;
