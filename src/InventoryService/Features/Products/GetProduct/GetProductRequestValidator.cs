using FluentValidation;

namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A validator for a <see cref="GetProductRequest"/>
/// </summary>
public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("ProductId must be a positive integer.");
    }
}
