using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// The Purchase Order request.
/// </summary>
/// <param name="PurchaseOrderId">The Purchase Order Id.</param>
public record GetPurchaseOrderRequest([FromQuery(Name = "purchaseOrderId")] string? PurchaseOrderId)
    : IRequest<Result<GetPurchaseOrderResponse>>;
