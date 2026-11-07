namespace Messaging;

/// <summary>
/// Describes an SNS topic and SQS queue pair used for message processing.
/// </summary>
public interface IQueueDescriptor
{
    /// <summary>
    /// Gets the name of the SNS topic associated with the queue.
    /// </summary>
    string TopicName { get; }

    /// <summary>
    /// Gets the name of the SQS queue.
    /// </summary>
    string QueueName { get; }
}

/// <summary>
/// Provides details for a SQS Consumer queue.
/// </summary>
/// <typeparam name="TQueueDescriptor">Describes the queue.</typeparam>
public sealed class QueueDetails<TQueueDescriptor>
    where TQueueDescriptor: class, IQueueDescriptor
{
    /// <summary>
    /// The URL of the queue after it has been created.
    /// </summary>
    public string QueueUrl => _queueUrl;

    /// <summary>
    /// Gets the name of the SNS topic associated with the queue.
    /// </summary>
    public string TopicName => _descriptor.TopicName;

    /// <summary>
    /// Gets the name of the SQS queue.
    /// </summary>
    public string QueueName => _descriptor.QueueName;

    private readonly string _queueUrl;

    private readonly TQueueDescriptor _descriptor;

    internal QueueDetails(TQueueDescriptor descriptor, string queueUrl)
    {
        _descriptor = descriptor;
        _queueUrl = queueUrl;
    }
}