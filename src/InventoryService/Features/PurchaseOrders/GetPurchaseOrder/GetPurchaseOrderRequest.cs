using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public record GetPurchaseOrderRequest([FromQuery(Name = "purchaseOrderId")] string? PurchaseOrderId)
    : IRequest<Result<GetPurchaseOrderResponse>>;
