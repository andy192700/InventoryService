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
/// <param name="CustomerId">Gets or sets the customer identifier.</param>
/// <param name="CountryId">Gets or sets the country identifier.</param>
/// <param name="lines">Gets or sets the collection of purchase order lines.</param>
public record CreatePurchaseOrderPayload(string? CustomerId, string? CountryId, ICollection<CreatePurchaseOrderPayloadLine>? lines);

/// <summary>
/// Represents a line item in a purchase order payload.
/// </summary>
/// <param name="ProductId">Gets or sets the product identifier.</param>
/// <param name="OrderQuantity">Gets or sets the quantity ordered.</param>
public record CreatePurchaseOrderPayloadLine(string? ProductId, string? OrderQuantity);
