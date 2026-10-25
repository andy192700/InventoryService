using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetPurchaseOrderDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetPurchaseOrderDBQuery> mockQuery,
        [Frozen] Mock<IGetPurchaseOrderResponseMapper> mockResponseMapper,
        GetPurchaseOrderRequest request,
        GetPurchaseOrderResponse response,
        GetPurchaseOrderDBResponse dbResponse,
        GetPurchaseOrderDBParameters dbParams,
        CancellationToken cancellation)
    {
        // Arrange
        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockQuery.Setup(x => x.GetPurchaseOrderAsync(dbParams, cancellation))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetPurchaseOrderHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().Be(response);

        mockQuery.Verify(x => x.GetPurchaseOrderAsync(dbParams, cancellation), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }


    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsNotFound(
        [Frozen] Mock<IGetPurchaseOrderDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetPurchaseOrderDBQuery> mockQuery,
        [Frozen] Mock<IGetPurchaseOrderResponseMapper> mockResponseMapper,
        GetPurchaseOrderDBParameters dbParams,
        GetPurchaseOrderDBResponse dbResponse,
        GetPurchaseOrderRequest request,
        GetPurchaseOrderResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        dbResponse.header = null;

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockQuery.Setup(x => x.GetPurchaseOrderAsync(dbParams, cancellation))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetPurchaseOrderHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<NotFoundError>();
        result.Errors[0].Message.Should().Be("Purchase Order not found");

        mockQuery.Verify(x => x.GetPurchaseOrderAsync(dbParams, cancellation), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Never);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }
}
