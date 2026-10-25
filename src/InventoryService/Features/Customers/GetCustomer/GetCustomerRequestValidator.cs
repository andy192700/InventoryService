using FluentValidation;

namespace InventoryService.Features.Customers.GetCustomer;

public class GetCustomerRequestValidator : AbstractValidator<GetCustomerRequest>
{
    public GetCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(x => ValidationHelpers.IsValidIntegerId(x))
            .WithMessage("CustomerId must be a positive integer.");
    }
}
