using FluentAssertions;
using InventoryService.Features.Customers.GetCustomers;

namespace InventoryService.UnitTests.Features.Customers.GetCustomers;

public class GetCustomersDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetCustomersRequest request)
    {
        // Arrange
        var sut = new GetCustomersDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.Filter.Should().Be(request.Filter);
    }
}
