using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Messaging.UnitTests;

public class QueueFactoryTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Create_CreatesAndSubscribesReturningQueueDetails(
        [Frozen] Mock<IAmazonSimpleNotificationService> mockSNS,
        [Frozen] Mock<IAmazonSQS> mockSQS,
        TestQueueDescriptor descriptor,
        TestLogger logger,
        string queueUrl,
        string topicArn)
    {
        // Arrange
        mockSNS.Setup(x => x.CreateTopicAsync(
                It.IsAny<CreateTopicRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTopicResponse
            {
                TopicArn = topicArn
            });

        mockSQS.Setup(x => x.CreateQueueAsync(
                It.IsAny<CreateQueueRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateQueueResponse
            {
                QueueUrl = queueUrl
            });

        mockSQS.Setup(x => x.GetQueueAttributesAsync(
                It.IsAny<GetQueueAttributesRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueueAttributesResponse());

        var sut = new QueueFactory<TestQueueDescriptor>(
            mockSNS.Object,
            mockSQS.Object,
            descriptor,
            logger);

        // Act
        var result = await sut.Create();

        // Assert
        result.Should().NotBeNull();
        result.QueueUrl.Should().Be(queueUrl);
        result.QueueName.Should().Be(descriptor.QueueName);
        result.TopicName.Should().Be(descriptor.TopicName);

        mockSNS.Verify(x => x.CreateTopicAsync(
            It.Is<CreateTopicRequest>(r =>
                r.Name == descriptor.TopicName),
            It.IsAny<CancellationToken>()),
            Times.Once);

        mockSQS.Verify(x => x.CreateQueueAsync(
            It.Is<CreateQueueRequest>(r =>
                r.QueueName == descriptor.QueueName),
            It.IsAny<CancellationToken>()),
            Times.Once);

        mockSQS.Verify(x => x.GetQueueAttributesAsync(
            It.Is<GetQueueAttributesRequest>(r =>
                r.QueueUrl == queueUrl &&
                r.AttributeNames.Contains(QueueAttributeName.QueueArn)),
            It.IsAny<CancellationToken>()),
            Times.Once);

        mockSNS.Verify(x => x.SubscribeAsync(
            It.Is<SubscribeRequest>(r =>
                r.TopicArn == topicArn &&
                r.Protocol == "sqs"),
            It.IsAny<CancellationToken>()),
            Times.Once);

        logger.LoggedLevel.Should().Be(LogLevel.Information);
        logger.LoggedMessage.Should().NotBeNull();
        logger.LoggedMessage.Should().Be($"SNS Topic created: {topicArn}, with Queue URL: {queueUrl}");
    }

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";

        public string TopicName => "test-topic";
    }

    public sealed class TestLogger : ILogger<QueueFactory<TestQueueDescriptor>>
    {
        public LogLevel LoggedLevel { get; set; }

        public string? LoggedMessage { get; set; }

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
            LoggedLevel = logLevel;
            LoggedMessage = state?.ToString();
        }
    }
}