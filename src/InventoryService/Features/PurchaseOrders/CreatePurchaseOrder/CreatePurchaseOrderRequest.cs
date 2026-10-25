using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// A request to create a purchase order.
/// </summary>
public record CreatePurchaseOrderRequest([FromBody] CreatePurchaseOrderPayload Payload) : IRequest<Result<CreatePurchaseOrderResponse>>;

/// <summary>
/// Payload for creating a purchase order.
/// </summary>
public class CreatePurchaseOrderPayload
{
    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the country identifier.
    /// </summary>
    public string? CountryId { get; set; }

    /// <summary>
    /// Gets or sets the collection of purchase order lines.
    /// </summary>
    public ICollection<CreatePurchaseOrderPayloadLine>? lines { get; set; }
}

/// <summary>
/// Represents a line item in a purchase order payload.
/// </summary>
public class CreatePurchaseOrderPayloadLine
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity ordered.
    /// </summary>
    public string? OrderQuantity { get; set; }
}
