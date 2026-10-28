using FluentValidation;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// A validator for a <see cref="CreatePurchaseOrderRequest"/>
/// </summary>
public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
{
    public CreatePurchaseOrderRequestValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull();

        RuleFor(x => x.Payload)
            .SetValidator(new CreatePurchaseOrderPayloadValidator())
            .When(x => x.Payload is not null);
    }
}

/// <summary>
/// A validator for a <see cref="CreatePurchaseOrderPayload"/>
/// </summary>
public class CreatePurchaseOrderPayloadValidator : AbstractValidator<CreatePurchaseOrderPayload>
{
    public CreatePurchaseOrderPayloadValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("CustomerId must be a positive integer.");

        RuleFor(x => x.CountryId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("CountryId must be a positive integer.");

        RuleFor(x => x.lines)
            .NotEmpty();

        RuleForEach(x => x.lines)
            .SetValidator(new CreatePurchaseOrderPayloadLineValidator());
    }
}

/// <summary>
/// A validator for a <see cref="CreatePurchaseOrderPayloadLine"/>
/// </summary>
public class CreatePurchaseOrderPayloadLineValidator : AbstractValidator<CreatePurchaseOrderPayloadLine>
{
    public CreatePurchaseOrderPayloadLineValidator()
    {
        RuleFor(x => x.OrderQuantity)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("OrderQuantity must be a positive integer.");

        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("ProductId must be a positive integer.");
    }
}
