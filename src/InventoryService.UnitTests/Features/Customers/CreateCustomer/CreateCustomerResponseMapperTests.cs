using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;

namespace InventoryService.UnitTests.Features.Customers.CreateCustomer;

public class CreateCustomerResponseMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreateCustomerDBResponse dbResponse)
    {
        // Arrange
        var sut = new CreateCustomerResponseMapper();

        // Act
        var result = sut.Map(dbResponse);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(dbResponse.CustomerId);
    }
}
