using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Products.GetProduct;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.Products.GetProduct;

public class GetProductHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetProductDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetProductDBQuery> mockQuery,
        [Frozen] Mock<IGetProductResponseMapper> mockResponseMapper,
        GetProductDBParameters dbParams,
        GetProductDBResponse dbResponse,
        GetProductRequest request,
        GetProductResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockQuery.Setup(x => x.GetProductAsync(dbParams, cancellation))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetProductHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().Be(response);

        mockQuery.Verify(x => x.GetProductAsync(dbParams, cancellation), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsNotFound(
        [Frozen] Mock<IGetProductDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetProductDBQuery> mockQuery,
        [Frozen] Mock<IGetProductResponseMapper> mockResponseMapper,
        GetProductDBParameters dbParams,
        GetProductDBResponse dbResponse,
        GetProductRequest request,
        GetProductResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        dbResponse.Product = null;

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockQuery.Setup(x => x.GetProductAsync(dbParams, cancellation))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetProductHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellation);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<NotFoundError>();
        result.Errors[0].Message.Should().Be("Product not found");

        mockQuery.Verify(x => x.GetProductAsync(dbParams, cancellation), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Never);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }
}
