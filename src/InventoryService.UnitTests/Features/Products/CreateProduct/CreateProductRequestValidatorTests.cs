using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;

namespace InventoryService.UnitTests.Features.Products.CreateProduct;

public class CreateProductRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(CreateProductRequest request)
    {
        // Arrange
        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenPayloadIsNull(CreateProductRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            Payload = null!
        };

        var sut = new CreateProductRequestValidator();

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
    public void Validate_IsInValid_WhenProductNameIsEmpty(
        string? productName,
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductName = productName!;

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Product Name' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(null!)]
    public void Validate_IsInValid_WhenProductDescIsEmpty(
        string? productDesc,
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductDesc = productDesc!;

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Product Desc' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenProductNameIsTooLong(
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductName = new string('A', 101);

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'Product Name' must be 100 characters or fewer. You entered {testPayload.ProductName.Length} characters.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenProductDescIsTooLong(
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductDesc = new string('A', 256);

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'Product Desc' must be 255 characters or fewer. You entered {testPayload.ProductDesc.Length} characters.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenProductCountriesIsNotSupplied(
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductCountries = null;

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"'Product Countries' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenCountryIdsAreDuplicated(
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductCountries = 
        [
            new CreateProductRequestProductCountries
            {
                UnitPrice = "2.3",
                CountryId = "1"
            },
            new CreateProductRequestProductCountries
            {
                UnitPrice = "2.3",
                CountryId = "1"
            }
        ];

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"Requested Product Countries must have unique CountryIds.");
    }

    [Theory]
    [InlineAutoMoqData(0.0)]
    [InlineAutoMoqData(-1.0)]
    public void Validate_IsInValid_WhenUnitPriceIsInValid(
        double unitPrice,
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductCountries!.First().UnitPrice = unitPrice.ToString();

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"UnitPrice must be a positive double.");
    }

    [Theory]
    [InlineAutoMoqData(0)]
    [InlineAutoMoqData(-1)]
    public void Validate_IsInValid_WhenCountryIdIsInValid(
        int countryId,
        CreateProductRequest request)
    {
        // Arrange
        var testPayload = request.Payload;

        testPayload.ProductCountries!.First().CountryId = countryId.ToString();

        var testRequest = request with
        {
            Payload = testPayload!
        };

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"CountryId must be a positive integer.");
    }
}
