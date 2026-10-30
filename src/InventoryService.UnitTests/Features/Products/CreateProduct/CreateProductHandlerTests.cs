using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests.Features.Products.CreateProduct;

public class CreateProductHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<ICreateProductDBCommand> mockCommand,
        [Frozen] Mock<ICreateProductResponseMapper> mockResponseMapper,
        [Frozen] Mock<ICreateProductDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<ILocationBuilder> mockLocationBuilder,
        [Frozen] Mock<ILogger<CreateProductHandler>> mockLogger,
        CreateProductDBParameters dbParams,
        CreateProductRequest request,
        CreateProductResponse response,
        CreateProductDBResponse dbResponse,
        string location,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockCommand.Setup(x => x.CreateProductAsync(dbParams, cancellationToken))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockLocationBuilder.Setup(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()))
            .Returns(location);

        var sut = new CreateProductHandler(
            mockParamsMapper.Object,
            mockResponseMapper.Object,
            mockCommand.Object,
            mockLocationBuilder.Object,
            mockLogger.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ICreated>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        var asCreatedResult = result as Created<CreateProductResponse>;
        asCreatedResult.Should().NotBeNull();
        asCreatedResult.Location.Should().Be(location);

        mockCommand.Verify(x => x.CreateProductAsync(dbParams, cancellationToken), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
        mockLocationBuilder.Verify(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()), Times.Once);

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Created Product with ProductId = {dbResponse.ProductId}",
            Times.Once());
    }
}