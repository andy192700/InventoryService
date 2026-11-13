using AutoFixture.Xunit2;
using InventoryService.UnitTests;
using Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace WarehouseService.UnitTests;

public class PurchaseOrderConsumerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task ConsumeAsync_LogsAndProducesShipPurchaseOrderRequest(
        [Frozen] Mock<ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic>> producerMock,
        [Frozen] Mock<IWorkDuration> mockWorkDuration,
        [Frozen] Mock<ILogger<PurchaseOrderConsumer>> loggerMock,
        FulfilPurchaseOrderRequest request)
    {
        // Arrange
        var durationSeconds = 1;

        mockWorkDuration.Setup(x => x.GetDuration())
            .Returns(durationSeconds);

        var sut = new PurchaseOrderConsumerWrapper(producerMock.Object, mockWorkDuration.Object, loggerMock.Object);

        // Act
        await sut.ConsumeAsync(request, CancellationToken.None);

        // Assert
        loggerMock.VerifyLog(LogLevel.Information, $"Warehouse has received a new Purchase Order, id: {request.PurchaseOrderId}", Times.Once());

        loggerMock.VerifyLog(LogLevel.Information, $"Team is working hard to fulfill the purchase order. Estimated wait time: {durationSeconds} seconds.", Times.Once());

        producerMock.Verify(x => x.ProduceAsync(
            It.Is<ShipPurchaseOrderRequest>(r => r.PurchaseOrderId == request.PurchaseOrderId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private class PurchaseOrderConsumerWrapper : PurchaseOrderConsumer
    {
        public PurchaseOrderConsumerWrapper(
            ISNSProducer<ShipPurchaseOrderRequest, ShipPurchaseOrderTopic> producer,
            IWorkDuration workDuration,
            ILogger<PurchaseOrderConsumer> logger)
            : base(producer, workDuration, logger)
        {
        }

        public new Task ConsumeAsync(FulfilPurchaseOrderRequest payload, CancellationToken cancellationToken)
        {
            return base.ConsumeAsync(payload, cancellationToken);
        }
    }
}
