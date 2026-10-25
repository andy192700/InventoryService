using FluentAssertions;
using InventoryService.Features.Countries.GetCountries;

namespace InventoryService.UnitTests.Features.Countries.GetCountries;

public class GetCountriesResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetCountriesDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetCountriesResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(dbResponse.Countries.Count);

        result
            .All(x => dbResponse.Countries.Any(y => 
                   y.CountryCode == x.CountryCode 
                && y.CountryId == x.CountryId 
                && y.CurrencyCode == x.CurrencyCode))
            .Should().BeTrue();
    }
}
