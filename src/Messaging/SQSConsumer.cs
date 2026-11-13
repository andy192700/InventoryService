using System.Text.Json;

namespace Messaging;

/// <summary>
/// Payload class received by a <see cref="SQSConsumer{TPayload}"/> representing a wrapped SNS message envelope.
/// </summary>
internal sealed class SNSEnvelope
{
    /// <summary>
    /// The content of the payload message as a JSON string.
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// Defines the interface for a consumer that handles messages fetched from an SQS queue.
/// </summary>
public interface ISQSConsumer
{
    /// <summary>
    /// Processes the raw JSON body of an SQS message asynchronously.
    /// </summary>
    /// <param name="body">The JSON string representing the message body.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that returns <c>true</c> if the message was successfully handled; otherwise <c>false</c>.</returns>
    Task<bool> HandleAsync(string body, CancellationToken cancellationToken);
}

/// <summary>
/// Base abstract class implementing <see cref="ISQSConsumer"/> to simplify message handling logic.
/// </summary>
public abstract class SQSConsumer : ISQSConsumer
{
    /// <inheritdoc/>
    public Task<bool> HandleAsync(string body, CancellationToken cancellationToken)
    {
        return HandleInternalAsync(body, cancellationToken);
    }

    /// <summary>
    /// Handles the internal processing of the message payload.
    /// </summary>
    /// <param name="payload">The raw message body string to process.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that returns <c>true</c> if the message was successfully handled; otherwise <c>false</c>.</returns>
    protected abstract Task<bool> HandleInternalAsync(string payload, CancellationToken cancellationToken);
}

/// <summary>
/// Abstract base class for an SQS consumer type that deserializes message payloads of type <typeparamref name="TPayload"/>.
/// </summary>
/// <typeparam name="TPayload">The type of the payload to deserialize from the message.</typeparam>
public abstract class SQSConsumer<TPayload> : SQSConsumer
{
    /// <inheritdoc/>
    protected override Task<bool> HandleInternalAsync(string body, CancellationToken cancellationToken)
    {
        var payload = DeserializePayload(body);

        if (payload is null)
        {
            return Task.FromResult(false);
        }
        else
        {
            ConsumeAsync(payload, cancellationToken);
            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// Consumes the deserialized payload asynchronously.
    /// </summary>
    /// <param name="payload">The deserialized payload to process.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task ConsumeAsync(TPayload payload, CancellationToken cancellationToken);

    private static TPayload? DeserializePayload(string body)
    {
        var envelope = JsonSerializer.Deserialize<SNSEnvelope>(body);

        return envelope?.Message is null
            ? default
            : JsonSerializer.Deserialize<TPayload>(envelope.Message);
    }
}
