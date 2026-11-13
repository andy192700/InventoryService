using Amazon.SQS.Model;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Messaging.UnitTests;

public class SQSConsumerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAsync_DeserializesPayload_HandlesMessage(
        TestPayload payload,
        CancellationToken cancellationToken)
    {
        // Arrange
        var message = new Message
        {
            Body = JsonSerializer.Serialize(new SNSEnvelope
            {
                Message = JsonSerializer.Serialize(payload)
            })
        };

        var sut = new TestConsumer();

        // Act
        var result = await sut.HandleAsync(message.Body, cancellationToken);

        // Assert
        result.Should().BeTrue();
        sut.HandledPayload.Should().BeTrue();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAsync_CannotDeserializesPayload_DoesNotHandleMessage(
        CancellationToken cancellationToken)
    {
        // Arrange
        var message = new Message
        {
            Body = JsonSerializer.Serialize(new SNSEnvelope())
        };

        var sut = new TestConsumer();

        // Act
        var result = await sut.HandleAsync(JsonSerializer.Serialize(message), cancellationToken);

        // Assert
        result.Should().BeFalse();
        sut.HandledPayload.Should().BeFalse();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task ExecuteAsync_Throws_WhenPayloadIsNotValidJson(
        string message,
        CancellationToken cancellationToken)
    {
        // Arrange
        var sut = new TestConsumer();

        // Act
        var func = async () => await sut.HandleAsync(message, cancellationToken);

        // Assert
        await func.Should().ThrowAsync<JsonException>();
    }

    public sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";

        public string TopicName => "test-topic";
    }

    public sealed record TestPayload(
        string ProductId,
        int Quantity);

    public sealed class TestConsumer
        : SQSConsumer<TestPayload>
    {
        public bool HandledPayload { get; set; }

        protected override Task ConsumeAsync(
            TestPayload payload,
            CancellationToken cancellationToken)
        {
            HandledPayload = true;

            return Task.CompletedTask;
        }
    }
}