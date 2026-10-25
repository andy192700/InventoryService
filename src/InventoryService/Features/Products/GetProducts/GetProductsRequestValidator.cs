using FluentValidation;

namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// A validator for a <see cref="GetProductsRequest"/>
/// </summary>
public class GetProductsRequestValidator : AbstractValidator<GetProductsRequest>
{
    public GetProductsRequestValidator()
    {
        RuleFor(x => x.Filter)
            .MaximumLength(100)
            .When(x => x.Filter is not null);
    }
}
