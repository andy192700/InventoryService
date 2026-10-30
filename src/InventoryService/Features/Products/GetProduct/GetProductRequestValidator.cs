using FluentValidation;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// A validator for a <see cref="GetProductRequest"/>
/// </summary>
public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage($"{nameof(GetProductRequest.ProductId)} must be a positive integer.");
    }
}
