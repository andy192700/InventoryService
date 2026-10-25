using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Customers.GetCustomers;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.Customers.GetCustomers;

public class GetCustomersHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetCustomersDBQuery> mockQuery,
        [Frozen] Mock<IGetCustomersDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetCustomersResponseMapper> mockResponseMapper,
        GetCustomersRequest request,
        ICollection<GetCustomersResponse> response,
        GetCustomersDBResponse dbResponse,
        GetCustomersDBParameters dbParameters,
        CancellationToken cancellation)
    {
        // Arrange
        mockQuery.Setup(x => x.GetCustomersAsync(dbParameters, cancellation))
            .ReturnsAsync(dbResponse);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParameters);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetCustomersHandler(mockQuery.Object, mockResponseMapper.Object, mockParamsMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        mockQuery.Verify(x => x.GetCustomersAsync(dbParameters, cancellation), Times.Once);

        mockParamsMapper.Verify(x => x.Map(request), Times.Once);

        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
    }
}
