using FluentAssertions;
using InventoryService.Features.Products.GetProducts;

namespace InventoryService.UnitTests.Features.Products.GetProducts;

public class GetProductsRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(GetProductsRequest request)
    {
        // Arrange
        var sut = new GetProductsRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData(null!)]
    [InlineAutoMoqData("")]
    public void Validate_IsValid_WithEmptyFilter(
        string filter, 
        GetProductsRequest request)
    {
        // Arrange
        var testRequest = request with { Filter = filter };

        var sut = new GetProductsRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInvalid_FilterTooLong(GetProductsRequest request)
    {
        // Arrange
        var testRequest = request with { Filter = new string('A', 101) };

        var sut = new GetProductsRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result
            .Errors[0]
            .ErrorMessage
            .Should()
            .Be($"The length of 'Filter' must be 100 characters or fewer. You entered {testRequest.Filter.Length} characters.");
    }
}