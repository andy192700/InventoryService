using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Messaging;

/// <summary>
/// Polls for new SQS messages, feeding the associated consumer with messages recieved.
/// </summary>
/// <typeparam name="TQueueDescriptor">The queue registration which will drive the consumer.</typeparam>
public interface ISQSSource<TQueueDescriptor>
    where TQueueDescriptor : class, IQueueDescriptor
{
    /// <summary>
    /// The details of the queue.
    /// </summary>
    QueueDetails<TQueueDescriptor> QueueDetails { get; }

    /// <summary>
    /// Reads data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of messages recieved.</returns>
    IAsyncEnumerable<Message> ReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Completes the consumption of a message.
    /// </summary>
    /// <param name="receiptHandle">Identifier for the message.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A Task in which the operation will complete.</returns>
    Task CompleteAsync(string receiptHandle, CancellationToken cancellationToken);
}

/// <summary>
/// Polls for new SQS messages, feeding the associated consumer with messages recieved.
/// </summary>
/// <typeparam name="TQueueDescriptor">The queue registration which will drive the consumer.</typeparam>
internal sealed class SQSSource<TQueueDescriptor> : ISQSSource<TQueueDescriptor>
    where TQueueDescriptor : class, IQueueDescriptor
{
    /// <inheritdoc/>
    public QueueDetails<TQueueDescriptor> QueueDetails { get; private set; }

    private readonly IAmazonSQS _sqs;

    private readonly IOptions<AWSOptions> _options;

    private readonly ILogger<SQSSource<TQueueDescriptor>> _logger;

    internal SQSSource(
        QueueDetails<TQueueDescriptor> queueDetails,
        IAmazonSQS sqs,
        IOptions<AWSOptions> options,
        ILogger<SQSSource<TQueueDescriptor>> logger)
    {
        QueueDetails = queueDetails;
        _sqs = sqs;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Message> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ReceiveMessageResponse? response;

            try
            {
                response = await _sqs.ReceiveMessageAsync(
                    new ReceiveMessageRequest
                    {
                        QueueUrl = QueueDetails.QueueUrl,
                        MaxNumberOfMessages = _options.Value.MaxNumberOfMessages,
                        WaitTimeSeconds = _options.Value.WaitTimeSeconds
                    },
                    cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Failed receiving messages from queue.");

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);

                continue;
            }

            var messages = response.Messages;

            if (messages is not null)
            {
                foreach (var message in response.Messages)
                {
                    yield return message;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task CompleteAsync(
        string receiptHandle,
        CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object?>
            {
                ["ReceiptHandle"] = receiptHandle,
                ["Queue"] = QueueDetails.QueueName,
                ["Topic"] = QueueDetails.TopicName
            });

        try
        {
            await _sqs.DeleteMessageAsync(
                new DeleteMessageRequest
                {
                    QueueUrl = QueueDetails.QueueUrl,
                    ReceiptHandle = receiptHandle
                },
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Failed completing message.");
        }
    }
}
