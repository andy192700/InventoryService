using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

public record GetPurchaseOrdersRequest(
    [FromQuery(Name = "productId")] string? ProductId,
    [FromQuery(Name = "countryId")] string? CountryId,
    [FromQuery(Name = "customerId")] string? CustomerId
    ) : IRequest<Result<ICollection<GetPurchaseOrdersResponse>>>;
