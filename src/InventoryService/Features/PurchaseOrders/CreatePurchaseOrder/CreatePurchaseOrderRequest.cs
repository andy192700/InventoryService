using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

public record CreatePurchaseOrderRequest([FromBody] CreatePurchaseOrderPayload Payload) : IRequest<Result<CreatePurchaseOrderResponse>>;

public class CreatePurchaseOrderPayload
{
    public string? CustomerId { get; set; }

    public string? CountryId { get; set; }

    public ICollection<CreatePurchaseOrderPayloadLine>? lines { get; set; }
}

public class CreatePurchaseOrderPayloadLine
{
    public string? ProductId { get; set; }

    public string? OrderQuantity { get; set; }
}