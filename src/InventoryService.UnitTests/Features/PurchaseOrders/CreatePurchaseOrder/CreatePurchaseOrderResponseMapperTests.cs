using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.CreatePurchaseOrder;

public class CreatePurchaseOrderResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreatePurchaseOrderDBResponse dbResponse)
    {
        // Arrange
        var sut = new CreatePurchaseOrderResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.PurchaseOrderId.Should().Be(dbResponse.PurchaseOrderId);
    }
}
