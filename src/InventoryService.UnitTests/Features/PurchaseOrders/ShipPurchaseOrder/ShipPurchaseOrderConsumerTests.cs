using AutoFixture.Xunit2;
using InventoryService.Features.PurchaseOrders.ShipPurchaseOrder;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests.Features.PurchaseOrders.ShipPurchaseOrder;

public class ShipPurchaseOrderConsumerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task ConsumeAsync_LogsAndMarksAsShipped_WhenUpdateSucceeds(
        [Frozen] Mock<IShipPurchaseOrderDBCommand> dbCommandMock,
        [Frozen] Mock<ILogger<ShipPurchaseOrderConsumer>> loggerMock,
        ShipPurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        // Arrange
        dbCommandMock.Setup(x => x.MarkAsShippedAsync(request.PurchaseOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new ShipPurchaseOrderConsumerWrapper(dbCommandMock.Object, loggerMock.Object);

        // Act
        await sut.ConsumeAsync(request, cancellationToken);

        // Assert
        loggerMock.VerifyLog(LogLevel.Information, $"Warehouse has completed order {request.PurchaseOrderId}, marking as shipped.", Times.Once());
        loggerMock.VerifyLog(LogLevel.Information, $"PurchaseOrderId: {request.PurchaseOrderId}, marked as shipped.", Times.Once());

        dbCommandMock.Verify(x => x.MarkAsShippedAsync(request.PurchaseOrderId, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task ConsumeAsync_LogsAndMarksAsShipped_WhenUpdateFails(
        [Frozen] Mock<IShipPurchaseOrderDBCommand> dbCommandMock,
        [Frozen] Mock<ILogger<ShipPurchaseOrderConsumer>> loggerMock,
        ShipPurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        // Arrange
        dbCommandMock.Setup(x => x.MarkAsShippedAsync(request.PurchaseOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new ShipPurchaseOrderConsumerWrapper(dbCommandMock.Object, loggerMock.Object);

        // Act
        await sut.ConsumeAsync(request, cancellationToken);

        // Assert
        loggerMock.VerifyLog(LogLevel.Information, $"Warehouse has completed order {request.PurchaseOrderId}, marking as shipped.", Times.Once());
        loggerMock.VerifyLog(LogLevel.Information, $"Failed marking PurchaseOrderId: {request.PurchaseOrderId} as shipped.", Times.Once());

        dbCommandMock.Verify(x => x.MarkAsShippedAsync(request.PurchaseOrderId, cancellationToken), Times.Once);
    }

    public class ShipPurchaseOrderConsumerWrapper : ShipPurchaseOrderConsumer
    {
        public ShipPurchaseOrderConsumerWrapper(
            IShipPurchaseOrderDBCommand dbCommand,
            ILogger<ShipPurchaseOrderConsumer> logger) : base(dbCommand, logger) { }

        public new Task ConsumeAsync(ShipPurchaseOrderRequest request, CancellationToken cancellation)
        {
            return base.ConsumeAsync(request, cancellation);
        }
    }
}