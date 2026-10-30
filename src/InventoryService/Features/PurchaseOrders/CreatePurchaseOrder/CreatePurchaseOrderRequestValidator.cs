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
            .WithMessage($"{nameof(CreatePurchaseOrderRequest.Payload.CustomerId)} must be a positive integer.");

        RuleFor(x => x.CountryId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage($"{nameof(CreatePurchaseOrderRequest.Payload.CountryId)} must be a positive integer.");

        RuleFor(x => x.lines)
            .NotEmpty();

        RuleFor(x => x.lines)
            .Must(x => HaveUniqueProductIds(x!))
            .WithMessage($"Requested Purchase Order Lines must have unique {nameof(CreatePurchaseOrderPayloadLine.ProductId)}s.")
            .When(x => x.lines is not null);

        RuleForEach(x => x.lines)
            .SetValidator(new CreatePurchaseOrderPayloadLineValidator());
    }

    private static bool HaveUniqueProductIds(ICollection<CreatePurchaseOrderPayloadLine> lines)
    {
        var distinctCount = lines.Select(x => x.ProductId).Distinct().Count();

        return distinctCount == lines.Count;
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
            .WithMessage($"{nameof(CreatePurchaseOrderPayloadLine.OrderQuantity)} must be a positive integer.");

        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage($"{nameof(CreatePurchaseOrderPayloadLine.ProductId)} must be a positive integer.");
    }
}
