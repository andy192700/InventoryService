using FluentValidation;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

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

public class CreatePurchaseOrderPayloadValidator : AbstractValidator<CreatePurchaseOrderPayload>
{
    public CreatePurchaseOrderPayloadValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("CustomerId must be a positive integer.");

        RuleFor(x => x.CountryId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("CountryId must be a positive integer.");

        RuleFor(x => x.lines)
            .NotEmpty();

        RuleForEach(x => x.lines)
            .SetValidator(new CreatePurchaseOrderPayloadLineValidator());
    }
}

public class CreatePurchaseOrderPayloadLineValidator : AbstractValidator<CreatePurchaseOrderPayloadLine>
{
    public CreatePurchaseOrderPayloadLineValidator()
    {
        RuleFor(x => x.OrderQuantity)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("OrderQuantity must be a positive integer.");

        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("ProductId must be a positive integer.");
    }
}
