using FluentValidation;

namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// A validator for a <see cref="CreateCustomerRequest"/>
/// </summary>
public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull();

        RuleFor(x => x.Payload)
            .SetValidator(new CreateCustomerRequestPayloadValidator());
    }
}


/// <summary>
/// A validator for a <see cref="CreateCustomerRequestPayload"/>
/// </summary>
public class CreateCustomerRequestPayloadValidator : AbstractValidator<CreateCustomerRequestPayload>
{
    public CreateCustomerRequestPayloadValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => x.LastName is not null);
    }
}
