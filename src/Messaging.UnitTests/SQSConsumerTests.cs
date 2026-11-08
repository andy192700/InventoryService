using Amazon.SQS.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace Messaging.UnitTests;

public class SQSConsumerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task ExecuteAsync_DeserializesPayload_HandlesMessage_AndCompletesMessage(
        [Frozen] Mock<ISQSSource<TestQueueDescriptor>> mockSource,
        [Frozen] Mock<ILogger<SQSConsumer<TestPayload, TestQueueDescriptor>>> mockLogger,
        TestPayload payload,
        string receiptHandle,
        string queueUrl)
    {
        // Arrange
        mockSource.Setup(x => x.QueueDetails)
            .Returns(new QueueDetails<TestQueueDescriptor>(new TestQueueDescriptor(), queueUrl));

        var message = new Message
        {
            ReceiptHandle = receiptHandle,
            Body = JsonSerializer.Serialize(new SNSEnvelope
            {
                Message = JsonSerializer.Serialize(payload)
            })
        };

        mockSource
            .Setup(x => x.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(GetMessages(message));

        var sut = new TestConsumer(mockSource.Object, mockLogger.Object);

        // Act
        await sut.ExecuteForTestAsync();

        // Assert
        sut.HandledPayload.Should().NotBeNull();
        sut.HandledPayload.Should().BeEquivalentTo(payload);

        mockSource.Verify(x => x.CompleteAsync(receiptHandle, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static async IAsyncEnumerable<Message> GetMessages(
        params Message[] messages)
    {
        foreach (var message in messages)
        {
            yield return message;
        }

        await Task.CompletedTask;
    }

    public sealed record TestPayload(
        string ProductId,
        int Quantity);

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";

        public string TopicName => "test-topic";
    }

    public sealed class TestConsumer
        : SQSConsumer<TestPayload, TestQueueDescriptor>
    {
        public TestPayload? HandledPayload { get; private set; }

        public TestConsumer(
            ISQSSource<TestQueueDescriptor> source,
            ILogger<SQSConsumer<TestPayload, TestQueueDescriptor>> logger)
            : base(source, logger)
        {
        }

        public Task ExecuteForTestAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }

        protected override Task HandleAsync(
            TestPayload payload,
            CancellationToken cancellationToken)
        {
            HandledPayload = payload;

            return Task.CompletedTask;
        }
    }
}