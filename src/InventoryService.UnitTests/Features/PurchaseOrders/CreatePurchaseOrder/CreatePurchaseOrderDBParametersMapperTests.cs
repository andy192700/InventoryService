using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

namespace InventoryService.UnitTests.Features.PurchaseOrders.CreatePurchaseOrder;

public class CreatePurchaseOrderDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreatePurchaseOrderRequest request)
    {
        // Arrange
        var sut = new CreatePurchaseOrderDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.CountryId.Should().Be(int.Parse(request.Payload.CountryId!));
        result.CustomerId.Should().Be(int.Parse(request.Payload.CustomerId!));

        for (var i = 0; i < result.Lines.Rows.Count; i++)
        {
            request.Payload.lines!.Any(x =>
                   (result.Lines.Rows[i]["ProductId"].ToString()) == x.ProductId!.ToString()
                && (result.Lines.Rows[i]["OrderQuantity"].ToString()) == x.OrderQuantity).Should().BeTrue();
        }
    }
}
