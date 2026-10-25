using FluentValidation;

namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// A validator for a <see cref="GetCustomersRequest"/>
/// </summary>
public class GetCustomersRequestValidator : AbstractValidator<GetCustomersRequest>
{
    public GetCustomersRequestValidator()
    {
        RuleFor(x => x.Filter)
            .MaximumLength(100)
            .When(x => x.Filter is not null);
    }
}
