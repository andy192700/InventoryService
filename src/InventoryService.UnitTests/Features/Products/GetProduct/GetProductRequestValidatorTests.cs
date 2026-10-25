using FluentAssertions;
using InventoryService.Features.Products.GetProduct;

namespace InventoryService.UnitTests.Features.Products.GetProduct;

public class GetProductRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(GetProductRequest request)
    {
        // Arrange
        var sut = new GetProductRequestValidator();

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
        string productId,
        GetProductRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            ProductId = productId
        };

        var sut = new GetProductRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("ProductId must be a positive integer.");
    }
}
