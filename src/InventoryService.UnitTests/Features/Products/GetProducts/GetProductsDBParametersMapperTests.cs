using FluentAssertions;
using InventoryService.Features.Products.GetProducts;

namespace InventoryService.UnitTests.Features.Products.GetProducts;

public class GetProductsDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetProductsRequest request)
    {
        // Arrange
        var sut = new GetProductsDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.Filter.Should().Be(result.Filter);
    }
}
