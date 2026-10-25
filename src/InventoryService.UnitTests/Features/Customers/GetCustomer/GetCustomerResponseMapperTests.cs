using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;

namespace InventoryService.UnitTests.Features.Customers.GetCustomer;

public class GetCustomerResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(GetCustomerDBResponse dbResponse)
    {
        // Arrange
        var sut = new GetCustomerResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();

        result.CustomerId.Should().Be(dbResponse.CustomerId);
        result.CreatedDate.Should().Be(dbResponse.CreatedDate);
        result.FirstName.Should().Be(dbResponse.FirstName);
        result.LastName.Should().Be(dbResponse.LastName);
    }
}