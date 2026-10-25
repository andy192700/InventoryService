using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;

namespace InventoryService.UnitTests.Features.Customers.CreateCustomer;

public class CreateCustomerRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(CreateCustomerRequest request)
    {
        // Arrange
        var sut = new CreateCustomerRequestValidator();
        
        // Act
        var result = sut.Validate(request);
        
        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_AsPayloadIsNull(CreateCustomerRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            Payload = null!
        };

        var sut = new CreateCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Payload' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(null!)]
    public void Validate_IsInValid_AsFirstNameIsEmpty(
        string? firstName,
        CreateCustomerRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            Payload = new CreateCustomerRequestPayload
            {
                FirstName = firstName!,
                LastName = request.Payload.LastName
            }
        };

        var sut = new CreateCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'First Name' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_AsFirstNameIsTooLong(
        CreateCustomerRequest request)
    {
        // Arrange
        var firstName = new string('A', 101);

        var testRequest = request with
        {
            Payload = new CreateCustomerRequestPayload
            {
                FirstName = firstName,
                LastName = request.Payload.LastName
            }
        };

        var sut = new CreateCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'First Name' must be 100 characters or fewer. You entered {firstName.Length} characters.");
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(null!)]
    public void Validate_IsInValid_AsLastNameIsEmpty(
        string? lastName,
        CreateCustomerRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            Payload = new CreateCustomerRequestPayload
            {
                FirstName = request.Payload.FirstName,
                LastName = lastName!
            }
        };

        var sut = new CreateCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Last Name' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_AsLastNameIsTooLong(
        CreateCustomerRequest request)
    {
        // Arrange
        var lastName = new string('A', 101);

        var testRequest = request with
        {
            Payload = new CreateCustomerRequestPayload
            {
                FirstName = request.Payload.FirstName,
                LastName = lastName
            }
        };

        var sut = new CreateCustomerRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'Last Name' must be 100 characters or fewer. You entered {lastName.Length} characters.");
    }
}
