using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;

namespace InventoryService.UnitTests.Features.Products.CreateProduct;

public class CreateProductResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreateProductDBResponse dbResponse)
    {
        // Arrange
        var sut = new CreateProductResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(dbResponse.ProductId);
    }
}
