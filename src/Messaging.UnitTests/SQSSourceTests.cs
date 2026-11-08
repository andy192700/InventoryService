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
    public async Task ReadAsync_ReadsMessage_AndCompleteAsync_CompletesMessage(
        [Frozen] Mock<IAmazonSQS> mockSQS,
        TestLogger logger,
        AWSOptions awsOptions,
        string queueUrl,
        string receiptHandle,
        string body)
    {
        // Arrange
        var queueDetails = new QueueDetails<TestQueueDescriptor>(new TestQueueDescriptor(), queueUrl);

        var options = Options.Create(awsOptions);

        var message = new Message
        {
            ReceiptHandle = receiptHandle,
            Body = body
        };

        mockSQS.Setup(x => x.ReceiveMessageAsync(
                It.IsAny<ReceiveMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages =
                [
                    message
                ]
            });

        var sut = new SQSSource<TestQueueDescriptor>(
            queueDetails,
            mockSQS.Object,
            options,
            logger);

        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        var returnedMessage = await sut.ReadAsync(cancellationTokenSource.Token).FirstAsync(cancellationTokenSource.Token);

        cancellationTokenSource.Cancel();

        await sut.CompleteAsync(message.ReceiptHandle, CancellationToken.None);

        // Assert
        returnedMessage.Should().BeSameAs(message);

        mockSQS.Verify(x => x.ReceiveMessageAsync(
            It.Is<ReceiveMessageRequest>(r =>
                r.QueueUrl == queueDetails.QueueUrl &&
                r.MaxNumberOfMessages == options.Value.MaxNumberOfMessages &&
                r.WaitTimeSeconds == options.Value.WaitTimeSeconds),
            It.IsAny<CancellationToken>()),
            Times.Once);

        mockSQS.Verify(x => x.DeleteMessageAsync(
            It.Is<DeleteMessageRequest>(r =>
                r.QueueUrl == queueDetails.QueueUrl &&
                r.ReceiptHandle == message.ReceiptHandle),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";

        public string TopicName => "test-topic";
    }

    public sealed class TestLogger : ILogger<SQSSource<TestQueueDescriptor>>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }
}