using Amazon.SQS;
using Amazon.SQS.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Messaging.UnitTests;

public class SQSSourceTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task ExecuteAsync_ProcessesMessages_AndDeletesThem(
        [Frozen] Mock<IAmazonSQS> mockSQS,
        AWSOptions awsOptions,
        Mock<ISQSConsumer> mockConsumer,
        Mock<ISQSReader<TestQueueDescriptor>> mockReader,
        string queueUrl,
        string receiptHandle,
        string body)
    {
        // Arrange
        var queueDetails = new QueueDetails<TestQueueDescriptor>(new TestQueueDescriptor(), queueUrl);
        var options = Options.Create(awsOptions);
        var logger = new TestLogger<SQSSource<TestQueueDescriptor>>();

        var message = new Message
        {
            ReceiptHandle = receiptHandle,
            Body = body,
            MessageId = "msg-1"
        };

        mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(ReturnMessagesAsync(message));

        mockConsumer.Setup(x => x.HandleAsync(message.Body, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new TestSource(queueDetails, mockSQS.Object, mockConsumer.Object, mockReader.Object, logger);

        using var cts = new CancellationTokenSource();

        cts.CancelAfter(1000);

        // Act
        await sut.ExecuteWorkerAsync(cts.Token);

        // Assert
        mockConsumer.Verify(x => x.HandleAsync(message.Body, It.IsAny<CancellationToken>()), Times.Once);

        mockSQS.Verify(x => x.DeleteMessageAsync(
            It.Is<DeleteMessageRequest>(req =>
                req.QueueUrl == queueDetails.QueueUrl &&
                req.ReceiptHandle == receiptHandle),
            It.IsAny<CancellationToken>()), Times.Once);

        logger.LogEntries.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData]
    internal async Task ExecuteAsync_LogsWarningAndErrors_AsExpected(
        [Frozen] Mock<IAmazonSQS> mockSQS,
        AWSOptions awsOptions,
        Mock<ISQSConsumer> mockConsumer,
        Mock<ISQSReader<TestQueueDescriptor>> mockReader,
        string queueUrl,
        string receiptHandle,
        string body)
    {
        // Arrange
        var queueDetails = new QueueDetails<TestQueueDescriptor>(new TestQueueDescriptor(), queueUrl);
        var options = Options.Create(awsOptions);
        var logger = new TestLogger<SQSSource<TestQueueDescriptor>>();

        var message = new Message
        {
            ReceiptHandle = receiptHandle,
            Body = body,
            MessageId = "msg-2"
        };

        mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(ReturnMessagesAsync(message));

        // Consumer returns false - triggers warning
        mockConsumer.Setup(x => x.HandleAsync(message.Body, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        mockSQS.Setup(x => x.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("Delete failed"));

        var sut = new TestSource(queueDetails, mockSQS.Object, mockConsumer.Object, mockReader.Object, logger);

        using var cts = new CancellationTokenSource();

        cts.CancelAfter(1000);

        // Act
        await sut.ExecuteWorkerAsync(cts.Token);

        // Assert
        mockConsumer.Verify(x => x.HandleAsync(message.Body, It.IsAny<CancellationToken>()), Times.Once);

        logger.LogEntries.Should().ContainSingle(log =>
            log.LogLevel == LogLevel.Warning &&
            log.Message.Contains("Unable to deserialize message payload."));

        logger.LogEntries.Should().ContainSingle(log =>
            log.LogLevel == LogLevel.Error &&
            log.Message.Contains("Failed completing message.") &&
            log.Exception != null &&
            log.Exception.Message == "Delete failed");
    }

    private async IAsyncEnumerable<Message> ReturnMessagesAsync(Message message)
    {
        yield return message;
        await Task.CompletedTask;
    }

    internal class TestSource : SQSSource<TestQueueDescriptor>
    {
        internal TestSource(
            QueueDetails<TestQueueDescriptor> queueDetails,
            IAmazonSQS sqs,
            ISQSConsumer consumer,
            ISQSReader<TestQueueDescriptor> reader,
            ILogger<SQSSource<TestQueueDescriptor>> logger)
            : base(queueDetails, sqs, consumer, reader, logger)
        {
        }

        public Task ExecuteWorkerAsync(CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(cancellationToken);
        }
    }

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";
        public string TopicName => "test-topic";
    }
}
