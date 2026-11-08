using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Messaging;

/// <summary>
/// Defines the contract for publishing messages to an SNS topic.
/// </summary>
/// <typeparam name="TMessage">
/// The type of message being published.
/// </typeparam>
/// <typeparam name="TTopicDescriptor">
/// The descriptor that identifies the SNS topic to publish to.
/// </typeparam>
public interface ISNSProducer<TMessage, TTopicDescriptor>
    where TMessage : class
    where TTopicDescriptor : ITopicDescriptor
{
    /// <summary>
    /// Publishes a message to the associated SNS topic.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task ProduceAsync(
        TMessage message,
        CancellationToken cancellationToken);
}

/// <summary>
/// Implementation of <see cref="ISNSProducer{TMessage, TTopicDescriptor}"/>
/// for publishing messages to SNS.
/// </summary>
/// <typeparam name="TMessage">
/// The type of message being published.
/// </typeparam>
/// <typeparam name="TTopicDescriptor">
/// The descriptor that identifies the SNS topic to publish to.
/// </typeparam>
internal sealed class SNSProducer<TMessage, TTopicDescriptor> : ISNSProducer<TMessage, TTopicDescriptor>
    where TMessage : class
    where TTopicDescriptor : ITopicDescriptor
{
    private TTopicDescriptor _topicDescriptor;

    private readonly IAmazonSimpleNotificationService _sns;

    private readonly IOptions<AWSOptions> _awsOptions;

    public SNSProducer(
        IAmazonSimpleNotificationService sns,
        TTopicDescriptor topicDescriptor,
        IOptions<AWSOptions> awsOptions)
    {
        _sns = sns;
        _topicDescriptor = topicDescriptor;
        _awsOptions = awsOptions;
    }

    /// <inheritdoc/>
    public async Task ProduceAsync(
        TMessage message,
        CancellationToken cancellationToken)
    {
        var topicArn = $"arn:aws:sns:{_awsOptions.Value.Region}:{_awsOptions.Value.AccountId}:{_topicDescriptor.TopicName}";

        await _sns.PublishAsync(
            new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonSerializer.Serialize(message),
                Subject = _topicDescriptor.TopicName
            },
            cancellationToken);
    }
}