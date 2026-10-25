using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetPurchaseOrderDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetPurchaseOrderResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.PurchaseOrderId.Should().Be(dbResponse.header!.PurchaseOrderId);
        result.OrderDate.Should().Be(dbResponse.header.OrderDate);
        result.CustomerId.Should().Be(dbResponse.header.CustomerId);
        result.CountryId.Should().Be(dbResponse.header.CountryId);

        foreach (var line in result.PurchaseOrderLines)
        {
            dbResponse.lines!.Any(x => 
                x.ProductId == line.ProductId
                && x.RowNo == line.RowNo
                && x.OrderQuantity == line.OrderQuantity
                && x.PurchaseOrderLineId == line.PurchaseOrderLineId).Should().BeTrue();
        }
    }
}
