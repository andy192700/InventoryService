using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace Messaging;

/// <summary>
/// An abstract factory which creates SQS consumers, returning a descriptor which describes the consumer.
/// </summary>
/// <typeparam name="TQueueDescriptor">Describes the queue.</typeparam>
internal interface IQueueFactory<TQueueDescriptor>
        where TQueueDescriptor : class, IQueueDescriptor
{
    /// <summary>
    /// Create the queue, returning the details.
    /// </summary>
    /// <returns>The details of the queue.</returns>
    Task<QueueDetails<TQueueDescriptor>> Create();
}

/// <summary>
/// An abstract factory which creates SQS consumers, returning a descriptor which describes the consumer.
/// </summary>
/// <typeparam name="TQueueDescriptor"></typeparam>
internal sealed class QueueFactory<TQueueDescriptor> : IQueueFactory<TQueueDescriptor>
        where TQueueDescriptor : class, IQueueDescriptor
{
    private readonly IAmazonSimpleNotificationService _sns;

    private readonly IAmazonSQS _sqs;

    private readonly TQueueDescriptor _queueDescriptor;

    private readonly ILogger<QueueFactory<TQueueDescriptor>> _logger;

    private const string SQSSubscribeProtocol = "sqs";

    public QueueFactory(
        IAmazonSimpleNotificationService sns,
        IAmazonSQS sqs,
        TQueueDescriptor queueDescriptor,
        ILogger<QueueFactory<TQueueDescriptor>> logger)
    {
        _sns = sns;
        _sqs = sqs;
        _queueDescriptor = queueDescriptor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<QueueDetails<TQueueDescriptor>> Create()
    {
        var topic = await _sns.CreateTopicAsync(
            new CreateTopicRequest
            {
                Name = _queueDescriptor.TopicName
            });

        var queue = await _sqs.CreateQueueAsync(
            new CreateQueueRequest
            {
                QueueName = _queueDescriptor.QueueName
            });

        var attributes = await _sqs.GetQueueAttributesAsync(
            new GetQueueAttributesRequest
            {
                QueueUrl = queue.QueueUrl,
                AttributeNames =
                [
                    QueueAttributeName.QueueArn
                ]
            });

        await _sns.SubscribeAsync(
            new SubscribeRequest
            {
                TopicArn = topic.TopicArn,
                Protocol = SQSSubscribeProtocol,
                Endpoint = attributes.QueueARN
            });

        _logger.LogInformation($"SNS Topic created: {topic.TopicArn}, with Queue URL: {queue.QueueUrl}");

        return new(_queueDescriptor, queue.QueueUrl);
    }
}
