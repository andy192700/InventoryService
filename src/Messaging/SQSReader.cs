using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Messaging;

/// <summary>
/// Reader whos job is to poll for new messages which shall be fed to a consumer.
/// </summary>
/// <typeparam name="TQueueDescriptor">Describes the queue.</typeparam>
public interface ISQSReader<TQueueDescriptor>
    where TQueueDescriptor : class, IQueueDescriptor
{
    /// <summary>
    /// Returns data asynchronously as it arrives.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of messages.</returns>
    IAsyncEnumerable<Message> ReadAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Reader whos job is to poll for new messages which shall be fed to a consumer.
/// </summary>
/// <typeparam name="TQueueDescriptor">Describes the queue.</typeparam>
internal class SQSReader<TQueueDescriptor> : ISQSReader<TQueueDescriptor>
    where TQueueDescriptor : class, IQueueDescriptor
{
    private QueueDetails<TQueueDescriptor> _queueDetails;

    private readonly IAmazonSQS _sqs;

    private readonly IOptions<AWSOptions> _options;

    private readonly ILogger<SQSReader<TQueueDescriptor>> _logger;

    public SQSReader(
        QueueDetails<TQueueDescriptor> queueDetails,
        IAmazonSQS sqs,
        IOptions<AWSOptions> options,
        ILogger<SQSReader<TQueueDescriptor>> logger)
    {
        _queueDetails = queueDetails;
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
                        QueueUrl = _queueDetails.QueueUrl,
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
}