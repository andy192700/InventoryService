using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrders;

public class GetPurchaseOrdersHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetPurchaseOrdersDBQuery> mockQuery,
        [Frozen] Mock<IGetPurchaseOrdersResponseMapper> mockResponseMapper,
        [Frozen] Mock<IGetPurchaseOrdersDBParametersMapper> mockParamsMapper,
        GetPurchaseOrdersRequest request,
        ICollection<GetPurchaseOrdersResponse> response,
        GetPurchaseOrdersDBResponse dbResponse,
        GetPurchaseOrdersDBParameters dbParams,
        CancellationToken cancellation)
    {
        // Arrange
        mockQuery.Setup(x => x.GetPurchaseOrdersAsync(dbParams, cancellation))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        var sut = new GetPurchaseOrdersHandler(mockQuery.Object, mockResponseMapper.Object, mockParamsMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        mockQuery.Verify(x => x.GetPurchaseOrdersAsync(dbParams, cancellation), Times.Once);

        mockParamsMapper.Verify(x => x.Map(request), Times.Once);

        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
    }
}
