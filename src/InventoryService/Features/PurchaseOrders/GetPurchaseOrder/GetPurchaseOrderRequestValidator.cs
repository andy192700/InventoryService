using FluentValidation;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// A validator for a <see cref="GetPurchaseOrderRequest"/>
/// </summary>
public class GetPurchaseOrderRequestValidator : AbstractValidator<GetPurchaseOrderRequest>
{
    public GetPurchaseOrderRequestValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("PurchaseOrderId must be a positive integer.");
    }
}
