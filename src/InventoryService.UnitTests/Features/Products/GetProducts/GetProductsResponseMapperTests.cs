using FluentAssertions;
using InventoryService.Features.Products.GetProducts;

namespace InventoryService.UnitTests.Features.Products.GetProducts;

public class GetProductsResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetProductsDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetProductsResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(dbResponse.Products.Count);

        foreach (var product in result)
        {
            dbResponse.Products.Any(x => 
                   x.ProductId == product.ProductId
                && x.CreatedDate == product.CreatedDate
                && x.ProductName == product.ProductName
                && x.ProductDesc == product.ProductDesc).Should().BeTrue();

            foreach (var productCountry in product.ProductCountries)
            {
                dbResponse.ProductCountries.Any(x =>
                       x.ProductId == product.ProductId
                    && x.UnitPrice == productCountry.UnitPrice
                    && x.CountryId == productCountry.CountryId).Should().BeTrue();
            }
        }
    }
}
