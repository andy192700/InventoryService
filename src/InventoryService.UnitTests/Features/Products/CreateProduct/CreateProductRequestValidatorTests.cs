using AutoFixture;
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
        string productDesc)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    )
                ]
            )
        );

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
        string productName)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    )
                ]
            )
        );

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
        string? productDesc)
    {
        // Arrange
        var productName = new string('A', 101);

        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    )
                ]
            )
        );

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'Product Name' must be 100 characters or fewer. You entered {productName.Length} characters.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenProductDescIsTooLong(
        string? productName)
    {
        // Arrange
        var productDesc = new string('A', 256);

        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    )
                ]
            )
        );

        var sut = new CreateProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be($"The length of 'Product Desc' must be 255 characters or fewer. You entered {productDesc.Length} characters.");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid_WhenProductCountriesIsNotSupplied(
        string productName,
        string productDesc)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                default
            )
        );
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
        string? productName,
        string productDesc)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    ),
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: "2.3",
                        CountryId: "1"
                    )
                ]
            )
        );

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
        string productName,
        string productDesc)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: unitPrice.ToString(),
                        CountryId: "1"
                    )
                ]
            )
        );

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
        double unitPrice,
        string productName,
        string productDesc)
    {
        // Arrange
        var testRequest = new CreateProductRequest
        (
            new CreateProductRequestPayload
            (
                productName,
                productDesc,
                [
                    new CreateProductRequestProductCountry
                    (
                        UnitPrice: unitPrice.ToString(),
                        CountryId: countryId.ToString()
                    )
                ]
            )
        );

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
