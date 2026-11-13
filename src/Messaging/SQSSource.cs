using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging;

/// <summary>
/// Polls for new SQS messages, feeding the associated consumer with messages recieved.
/// </summary>
/// <typeparam name="TQueueDescriptor">The queue registration which will drive the consumer.</typeparam>
internal class SQSSource<TQueueDescriptor> : BackgroundService
    where TQueueDescriptor : class, IQueueDescriptor
{
    /// <inheritdoc/>
    private QueueDetails<TQueueDescriptor> _queueDetails;

    private readonly IAmazonSQS _sqs;

    private readonly ISQSConsumer _consumer;

    private readonly ISQSReader<TQueueDescriptor> _reader;

    private readonly ILogger<SQSSource<TQueueDescriptor>> _logger;

    internal SQSSource(
        QueueDetails<TQueueDescriptor> queueDetails,
        IAmazonSQS sqs,
        ISQSConsumer consumer,
        ISQSReader<TQueueDescriptor> reader,
        ILogger<SQSSource<TQueueDescriptor>> logger)
    {
        _queueDetails = queueDetails;
        _sqs = sqs;
        _consumer = consumer;
        _reader = reader;
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
        await foreach (var message in _reader.ReadAsync(cancellationToken))
        {
            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["MessageId"] = message.MessageId,
                    ["ReceiptHandle"] = message.ReceiptHandle,
                    ["Queue"] = _queueDetails.QueueName,
                    ["Topic"] = _queueDetails.TopicName
                });

            try
            {
                var consumptionResult = await _consumer.HandleAsync(message.Body, cancellationToken);

                if (!consumptionResult)
                {
                    _logger.LogWarning(
                        "Unable to deserialize message payload.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed processing message.");
            }
            finally
            {
                try
                {
                    await _sqs.DeleteMessageAsync(
                        new DeleteMessageRequest
                        {
                            QueueUrl = _queueDetails.QueueUrl,
                            ReceiptHandle = message.ReceiptHandle
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
    }
}
