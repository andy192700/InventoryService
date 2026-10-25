using FluentValidation;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// A validator for a <see cref="CreateProductRequest"/>
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull();

        RuleFor(x => x.Payload)
            .SetValidator(new CreateProductRequestPayloadValidator())
            .When(x => x.Payload is not null);
    }
}

/// <summary>
/// A validator for a <see cref="CreateProductRequestPayload"/>
/// </summary>
public class CreateProductRequestPayloadValidator : AbstractValidator<CreateProductRequestPayload>
{
    public CreateProductRequestPayloadValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty();

        RuleFor(x => x.ProductName)
            .MaximumLength(100)
            .When(x => x.ProductName is not null);

        RuleFor(x => x.ProductDesc)
            .NotEmpty();

        RuleFor(x => x.ProductDesc)
            .MaximumLength(255)
            .When(x => x.ProductName is not null);

        RuleFor(x => x.ProductCountries)
            .NotEmpty();

        RuleForEach(x => x.ProductCountries)
            .SetValidator(new CreateProductRequestProductCountriesValidator());
    }
}

/// <summary>
/// A validator for a <see cref="CreateProductRequestProductCountries"/>
/// </summary>
public class CreateProductRequestProductCountriesValidator : AbstractValidator<CreateProductRequestProductCountries>
{
    public CreateProductRequestProductCountriesValidator()
    {
        RuleFor(x => x.CountryId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("CountryId must be a positive integer.");

        RuleFor(x => x.UnitPrice)
            .Must(x => ValidationHelpers.IsValidDoubleId(x))
            .WithMessage("UnitPrice must be a positive double.");
    }
}