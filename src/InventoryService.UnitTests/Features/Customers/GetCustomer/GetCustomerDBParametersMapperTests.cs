using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;

namespace InventoryService.UnitTests.Features.Customers.GetCustomer;

public class GetCustomerDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetCustomerRequest request)
    {
        // Arrange
        var sut = new GetCustomerDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.ToString().Should().Be(request.CustomerId);
    }
}
