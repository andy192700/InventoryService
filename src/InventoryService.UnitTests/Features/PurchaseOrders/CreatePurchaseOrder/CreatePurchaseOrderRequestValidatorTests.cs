using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.CreatePurchaseOrder;

public class CreatePurchaseOrderRequestValidatorTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Validate_IsValid(CreatePurchaseOrderRequest request)
    {
        // Arrange
        var sut = new CreatePurchaseOrderRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_IsInValid()
    {
        // Arrange
        var lines = new List<CreatePurchaseOrderPayloadLine>()
            {
                new CreatePurchaseOrderPayloadLine
                {
                    OrderQuantity = "abc",
                    ProductId = "abc"
                }
            };

        var request = new CreatePurchaseOrderRequest
        (
            new CreatePurchaseOrderPayload
            {
                CustomerId = "abc",
                CountryId = "abc",
                lines = lines
            }
        );

        var sut = new CreatePurchaseOrderRequestValidator();

        // Act
        var result = sut.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(4);
        result.Errors[0].ErrorMessage.Should().Be("CustomerId must be a positive integer.");
        result.Errors[1].ErrorMessage.Should().Be("CountryId must be a positive integer.");
        result.Errors[2].ErrorMessage.Should().Be("OrderQuantity must be a positive integer.");
        result.Errors[3].ErrorMessage.Should().Be("ProductId must be a positive integer.");
    }
}
