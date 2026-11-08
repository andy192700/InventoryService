using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using Xunit;

namespace Messaging.UnitTests;

public class SNSProducerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task ProduceAsync_PublishesMessageToConfiguredTopic(
        [Frozen] Mock<IAmazonSimpleNotificationService> mockSNS,
        TestTopicDescriptor topicDescriptor,
        TestMessage message,
        AWSOptions awsOptions)
    {
        // Arrange
        var options = Options.Create(awsOptions);

        var sut = new SNSProducer<TestMessage, TestTopicDescriptor>(
            mockSNS.Object,
            topicDescriptor,
            options);

        // Act
        await sut.ProduceAsync(
            message,
            CancellationToken.None);

        // Assert
        mockSNS.Verify(x => x.PublishAsync(
            It.Is<PublishRequest>(r =>
                r.TopicArn == $"arn:aws:sns:{awsOptions.Region}:{awsOptions.AccountId}:{topicDescriptor.TopicName}" &&
                r.Subject == topicDescriptor.TopicName &&
                r.Message == JsonSerializer.Serialize(message)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    public sealed record TestMessage(
        string ProductId,
        int Quantity);

    public sealed class TestTopicDescriptor : ITopicDescriptor
    {
        public string TopicName => "test-topic";
    }
}