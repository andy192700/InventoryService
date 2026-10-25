using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;

namespace InventoryService.UnitTests.Features.Customers.GetCustomer;

public class GetCustomerRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(GetCustomerRequest request)
    {
        // Arrange
        var sut = new GetCustomerRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData("I am a string")]
    public void Validate_IsInValid(
        string customerId,
        GetCustomerRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            CustomerId = customerId
        };

        var sut = new GetCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("CustomerId must be a positive integer.");
    }
}
