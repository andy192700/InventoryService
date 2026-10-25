using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetPurchaseOrdersDBResponse dBResponse)
    {
        // Arrange
        var sut = new GetPurchaseOrdersResponseMapper();

        // Act
        var result = sut.Map(dBResponse);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(dBResponse.Headers.Count);

        foreach (var purchaseOrder in result)
        {
            dBResponse.Headers.Any(x =>
                x.PurchaseOrderId == purchaseOrder.PurchaseOrderId
                && x.OrderDate == purchaseOrder.OrderDate
                && x.CustomerId == purchaseOrder.CustomerId
                && x.CountryId == purchaseOrder.CountryId).Should().BeTrue();

            foreach (var purchaseOrderLine in purchaseOrder.PurchaseOrderLines)
            {
                dBResponse.Lines.Any(x =>
                x.PurchaseOrderLineId == purchaseOrderLine.PurchaseOrderLineId
                && x.PurchaseOrderId == purchaseOrder.PurchaseOrderId
                && x.ProductId == purchaseOrderLine.ProductId
                && x.OrderQuantity == purchaseOrderLine.OrderQuantity
                && x.RowNo == purchaseOrderLine.RowNo).Should().BeTrue();
            }
        }
    }
}
