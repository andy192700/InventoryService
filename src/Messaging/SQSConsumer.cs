using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Messaging;

/// <summary>
/// Payload class recieved by a <see cref="SQSConsumer{TPayload, TQueueDescriptor}"/>
/// </summary>
internal sealed class SNSEnvelope
{
    /// <summary>
    /// The content of the payload.
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// The basis for a SQS Consumer, which hides any complexity.
/// </summary>
/// <typeparam name="TPayload">The payload type of a message received by the consumer.</typeparam>
/// <typeparam name="TQueueDescriptor">The queue registration which will drive the consumer.</typeparam>
public abstract class SQSConsumer<TPayload, TQueueDescriptor> : BackgroundService
    where TPayload : class
    where TQueueDescriptor : class, IQueueDescriptor
{
    private readonly ISQSSource<TQueueDescriptor> _source;

    private readonly ILogger<SQSConsumer<TPayload, TQueueDescriptor>> _logger;

    protected SQSConsumer(
        ISQSSource<TQueueDescriptor> source,
        ILogger<SQSConsumer<TPayload, TQueueDescriptor>> logger)
    {
        _source = source;
        _logger = logger;
    }

    /// <summary>
    /// Holds and reads incoming messages.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A Task in which the operation will complete.</returns>
    protected override async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        await foreach (var message in _source.ReadAsync(cancellationToken))
        {
            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["MessageId"] = message.MessageId,
                    ["Queue"] = _source.QueueDetails.QueueName,
                    ["Topic"] = _source.QueueDetails.TopicName
                });

            try
            {
                var payload = DeserializePayload(message.Body);

                if (payload is null)
                {
                    _logger.LogWarning(
                        "Unable to deserialize message payload.");

                    continue;
                }

                await HandleAsync(
                    payload,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed processing message.");
            }
            finally
            {
                await _source.CompleteAsync(
                    message.ReceiptHandle,
                    cancellationToken);
            }
        }
    }

    /// <summary>
    /// Handles the implementation consumer's message consumption.
    /// </summary>
    /// <param name="payload">The payload of the message received by the consumer.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A Task in which the operation will complete.</returns>
    protected abstract Task HandleAsync(
        TPayload payload,
        CancellationToken cancellationToken);

    private static TPayload? DeserializePayload(string body)
    {
        var envelope = JsonSerializer.Deserialize<SNSEnvelope>(body);

        return envelope?.Message is null
            ? default
            : JsonSerializer.Deserialize<TPayload>(envelope.Message);
    }
}