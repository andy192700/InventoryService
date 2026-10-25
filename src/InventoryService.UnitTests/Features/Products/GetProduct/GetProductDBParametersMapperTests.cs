using FluentAssertions;
using InventoryService.Features.Products.GetProduct;

namespace InventoryService.UnitTests.Features.Products.GetProduct;

public class GetProductDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetProductRequest request)
    {
        // Arrange        
        var sut = new GetProductDBParametersMapper();
        
        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(int.Parse(request.ProductId!));
    }
}
