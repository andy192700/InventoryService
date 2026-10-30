using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests.Features.PurchaseOrders.CreatePurchaseOrder;

public class CreatePurchaseOrderHandlerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<ICreatePurchaseOrderDBCommand> mockCommand,
        [Frozen] Mock<ICreatePurchaseOrderResponseMapper> mockResponseMapper,
        [Frozen] Mock<ICreatePurchaseOrderDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<ILocationBuilder> mockLocationBuilder,
        [Frozen] Mock<ILogger<CreatePurchaseOrderHandler>> mockLogger,
        CreatePurchaseOrderDBParameters dbParams,
        CreatePurchaseOrderRequest request,
        CreatePurchaseOrderResponse response,
        CreatePurchaseOrderDBResponse dbResponse,
        string location,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockCommand.Setup(x => x.CreatePurchaseOrderAsync(dbParams, cancellationToken))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockLocationBuilder.Setup(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()))
            .Returns(location);

        var sut = new CreatePurchaseOrderHandler(
            mockParamsMapper.Object,
            mockCommand.Object,
            mockResponseMapper.Object,
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

        var asCreatedResult = result as Created<CreatePurchaseOrderResponse>;
        asCreatedResult.Should().NotBeNull();
        asCreatedResult.Location.Should().Be(location);

        mockCommand.Verify(x => x.CreatePurchaseOrderAsync(dbParams, cancellationToken), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
        mockLocationBuilder.Verify(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()), Times.Once);

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Created Purhase Order with PurchaseOrderId = {dbResponse.PurchaseOrderId}",
            Times.Once());
    }
}
