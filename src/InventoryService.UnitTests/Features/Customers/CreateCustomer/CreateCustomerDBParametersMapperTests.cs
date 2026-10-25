using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;

namespace InventoryService.UnitTests.Features.Customers.CreateCustomer;

public class CreateCustomerDBParametersMapperTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Map_MapsCorrectly(CreateCustomerRequest request)
    {
        // Arrange
        var sut = new CreateCustomerDBParametersMapper();

        // Act
        var result = sut.Map(request);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(request.Payload.FirstName);
        result.LastName.Should().Be(request.Payload.LastName);
    }
}
