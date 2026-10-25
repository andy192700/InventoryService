using FluentAssertions;
using InventoryService.Features.Customers.GetCustomers;

namespace InventoryService.UnitTests.Features.Customers.GetCustomers;

public class GetCustomersResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetCustomersDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetCustomersResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(dbResponse.Customers.Count);

        result
            .All(x => dbResponse.Customers.Any(y =>
                   y.FirstName == x.FirstName
                && y.LastName == x.LastName
                && y.CreatedDate == x.CreatedDate
                && y.CustomerId == x.CustomerId))
            .Should().BeTrue();
    }
}
