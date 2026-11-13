using Amazon.SQS;
using Amazon.SQS.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Messaging.UnitTests;

public class SQSReaderTests
{
    [Theory]
    [InlineAutoMoqData]
    internal async Task ReadAsync_YieldsMessagesFromSQS(
        [Frozen] Mock<IAmazonSQS> mockSQS,
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

        mockSQS.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages = new List<Message> { message }
            });

        var logger = new TestLogger<SQSReader<TestQueueDescriptor>>();
        var sut = new SQSReader<TestQueueDescriptor>(queueDetails, mockSQS.Object, options, logger);

        using var cts = new CancellationTokenSource();

        var messages = new List<Message>();

        // Act
        // Cancel after first message to avoid infinite loop
        cts.CancelAfter(1000);

        await foreach (var msg in sut.ReadAsync(cts.Token))
        {
            messages.Add(msg);
            cts.Cancel();
        }

        // Assert
        messages.Should().ContainSingle()
            .Which.Should().BeSameAs(message);

        mockSQS.Verify(x => x.ReceiveMessageAsync(
            It.Is<ReceiveMessageRequest>(r =>
                r.QueueUrl == queueDetails.QueueUrl &&
                r.MaxNumberOfMessages == options.Value.MaxNumberOfMessages &&
                r.WaitTimeSeconds == options.Value.WaitTimeSeconds),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Theory]
    [InlineAutoMoqData]
    internal async Task ReadAsync_LogsError_AndContinues_WhenExceptionThrown(
        [Frozen] Mock<IAmazonSQS> mockSQS,
        AWSOptions awsOptions,
        string queueUrl,
        string exceptionMessage)
    {
        // Arrange
        var queueDetails = new QueueDetails<TestQueueDescriptor>(new TestQueueDescriptor(), queueUrl);
        var options = Options.Create(awsOptions);

        int callCount = 0;

        mockSQS.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (callCount++ == 0)
                {
                    throw new System.Exception(exceptionMessage);
                }
                else
                {
                    return new ReceiveMessageResponse { Messages = new List<Message>() };
                }
            });

        var logger = new TestLogger<SQSReader<TestQueueDescriptor>>();
        var sut = new SQSReader<TestQueueDescriptor>(queueDetails, mockSQS.Object, options, logger);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        // Act
        await foreach (var _ in sut.ReadAsync(cts.Token))
        {
            // No messages expected
        }

        // Assert
        logger.LogEntries.Should().ContainSingle(log =>
            log.LogLevel == LogLevel.Error &&
            log.Message.Contains("Failed receiving messages from queue.") &&
            log.Exception != null &&
            log.Exception.Message == exceptionMessage);
    }

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";
        public string TopicName => "test-topic";
    }
}
