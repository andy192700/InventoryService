using Microsoft.Extensions.Options;

namespace Messaging;

/// <summary>
/// Configuration options used to connect to AWS services such as SNS and SQS.
/// </summary>
public sealed class AWSOptions
{
    /// <summary>
    /// Gets or sets the AWS account identifier used when constructing resource ARNs.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    /// Gets the service endpoint URL used to access AWS services.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets the AWS region in which messaging resources are located.
    /// </summary>
    public required string Region { get; init; }

    /// <summary>
    /// Gets the AWS access key used for authentication.
    /// </summary>
    public required string AccessKey { get; init; }

    /// <summary>
    /// Gets the AWS secret key used for authentication.
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of messages to retrieve from SQS in a single poll operation.
    /// </summary>
    public int MaxNumberOfMessages { get; set; }

    /// <summary>
    /// Gets or sets the number of seconds that SQS long polling should wait for messages.
    /// </summary>
    public int WaitTimeSeconds { get; set; }
}

/// <summary>
/// Validate <see cref="AWSOptions"/> object when supplied as configuration.
/// </summary>
internal sealed class AWSOptionsValidator : IValidateOptions<AWSOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, AWSOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AccessKey))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.AccessKey)} must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.Region))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.Region)} must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.Url)} must be configured.");
        }

        if (!Uri.TryCreate(
                options.Url,
                UriKind.Absolute,
                out _))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.Url)} must be a valid absolute URI.");
        }

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.SecretKey)} must be configured.");
        }

        if (options.MaxNumberOfMessages <= 0)
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.MaxNumberOfMessages)} must be configured.");
        }

        if (options.WaitTimeSeconds <= 0)
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.MaxNumberOfMessages)} must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.AccountId))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AWSOptions.AccountId)} must be configured.");
        }

        return ValidateOptionsResult.Success;
    }
}