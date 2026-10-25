using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Valid_IsValid(GetPurchaseOrderRequest request)
    {
        // Arrange
        var sut = new GetPurchaseOrderRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    public void Valid_IsInValid_WhenPurchaseOrderIdIsInvalid(
        GetPurchaseOrderRequest request)
    {
        // Arrange
        var testRequest = request with { PurchaseOrderId = "abc" };

        var sut = new GetPurchaseOrderRequestValidator();

        // Act
        var result = sut.Validate(testRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("PurchaseOrderId must be a positive integer.");
    }
}
