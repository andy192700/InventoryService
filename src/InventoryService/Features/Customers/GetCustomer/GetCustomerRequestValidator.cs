using FluentValidation;

namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// A validator for a <see cref="GetCustomerRequest"/>
/// </summary>
public class GetCustomerRequestValidator : AbstractValidator<GetCustomerRequest>
{
    public GetCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(x => ValidationHelpers.IsValidInteger(x))
            .WithMessage("CustomerId must be a positive integer.");
    }
}
