using FluentValidation;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

/// <summary>
/// A validator for a <see cref="GetPurchaseOrderRequest"/>
/// </summary>
public class GetPurchaseOrderRequestValidator : AbstractValidator<GetPurchaseOrderRequest>
{
    public GetPurchaseOrderRequestValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("PurchaseOrderId must be a positive integer.");
    }
}
