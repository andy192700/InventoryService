using FluentAssertions;
using InventoryService.Features.Products.GetProduct;

namespace InventoryService.UnitTests.Features.Products.GetProduct;

public class GetProductResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetProductDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetProductResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.CreatedDate.Should().Be(dbResponse.Product!.CreatedDate);
        result.ProductId.Should().Be(dbResponse.Product.ProductId);
        result.ProductName.Should().Be(dbResponse.Product.ProductName);
        result.ProductDesc.Should().Be(dbResponse.Product.ProductDesc);
        result.ProductCountries.Count.Should().Be(dbResponse.ProductCountries.Count);
        result.ProductCountries.All(x => dbResponse.ProductCountries.Any(y =>
               x.UnitPrice == y.UnitPrice
            && x.CountryId == y.CountryId)).Should().BeTrue();
    }
}
