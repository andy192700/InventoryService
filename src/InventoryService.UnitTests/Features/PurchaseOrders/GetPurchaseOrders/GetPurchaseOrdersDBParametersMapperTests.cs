using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetPurchaseOrdersRequest request)
    {
        // Arrange
        var sut = new GetPurchaseOrdersDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.ToString().Should().Be(request.ProductId);
        result.CountryId.ToString().Should().Be(request.CountryId);
        result.CustomerId.ToString().Should().Be(request.CustomerId);
    }
}
