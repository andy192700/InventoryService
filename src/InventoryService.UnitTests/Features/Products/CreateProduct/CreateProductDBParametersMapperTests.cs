using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;

namespace InventoryService.UnitTests.Features.Products.CreateProduct;

public class CreateProductDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreateProductRequest request)
    {
        // Arrange
        var sut = new CreateProductDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.ProductName.Should().Be(result.ProductName);
        result.ProductDesc.Should().Be(result.ProductDesc);

        for (var i = 0; i < result.ProductCountries.Rows.Count; i++)
        {
            request.Payload.ProductCountries!.Any(x =>
                   (result.ProductCountries.Rows[i]["CountryId"]).ToString() == x.CountryId
                && (result.ProductCountries.Rows[i]["UnitPrice"]).ToString() == x.UnitPrice).Should().BeTrue();
        }
    }
}
