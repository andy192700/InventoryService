using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetPurchaseOrderRequest request)
    {
        // Arrange
        var sut = new GetPurchaseOrderDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.PurchaseOrderId.Should().Be(int.Parse(request.PurchaseOrderId!));
    }
}
