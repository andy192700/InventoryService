using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Products.GetProducts;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.Products.GetProducts;

public class GetProductsHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetProductsDBQuery> mockQuery,
        [Frozen] Mock<IGetProductsResponseMapper> mockResponseMapper,
        [Frozen] Mock<IGetProductsDBParametersMapper> mockParamsMapper,
        GetProductsDBParameters dbParameters,
        GetProductsDBResponse dbResponse,
        GetProductsRequest request,
        ICollection<GetProductsResponse> response,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockQuery.Setup(x => x.GetProductsAsync(dbParameters, cancellationToken))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParameters);

        var sut = new GetProductsHandler(mockQuery.Object, mockResponseMapper.Object, mockParamsMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        mockQuery.Verify(x => x.GetProductsAsync(dbParameters, cancellationToken), Times.Once);

        mockParamsMapper.Verify(x => x.Map(request), Times.Once);

        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);

    }
}
