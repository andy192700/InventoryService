using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(GetPurchaseOrdersRequest request)
    {
        // Arrange
        var sut = new GetPurchaseOrdersRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsInValid(GetPurchaseOrdersRequest request)
    {
        // Arrange
        var testRequest = request with
        {
            ProductId = "abc",
            CustomerId = "abc",
            CountryId = "abc",
        };

        var sut = new GetPurchaseOrdersRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors[0].ErrorMessage.Should().Be("ProductId must be a positive integer.");
        result.Errors[1].ErrorMessage.Should().Be("CustomerId must be a positive integer.");
        result.Errors[2].ErrorMessage.Should().Be("CountryId must be a positive integer.");
    }
}
