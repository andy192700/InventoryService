using FluentValidation;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// A validator for a <see cref="GetPurchaseOrdersRequest"/>
/// </summary>
public class GetPurchaseOrdersRequestValidator : AbstractValidator<GetPurchaseOrdersRequest>
{
    public GetPurchaseOrdersRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidInteger(x!))
            .When(x => x.ProductId is not null)
            .WithMessage("ProductId must be a positive integer.");

        RuleFor(x => x.CustomerId)
            .Must(x => ValidationHelpers.IsValidInteger(x!))
            .When(x => x.CustomerId is not null)
            .WithMessage("CustomerId must be a positive integer.");

        RuleFor(x => x.CountryId)
            .Must(x => ValidationHelpers.IsValidInteger(x!))
            .When(x => x.CountryId is not null)
            .WithMessage("CountryId must be a positive integer.");
    }
}
