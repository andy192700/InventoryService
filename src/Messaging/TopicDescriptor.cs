namespace Messaging;

/// <summary>
/// Describes an SNS Topic.
/// </summary>
public interface ITopicDescriptor
{
    /// <summary>
    /// The Topic's name.
    /// </summary>
    string TopicName { get; }
}

/// <summary>
/// Describes an SNS Topic.
/// </summary>
public abstract class TopicDescriptor : ITopicDescriptor
{
    /// <inheritdoc/>
    public abstract string TopicName { get; }
}
