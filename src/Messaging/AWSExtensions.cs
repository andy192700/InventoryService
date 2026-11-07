using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging;

public static class AWSExtensions
{
    /// <summary>
    /// Adds required SQS and SNS dependencies in order to allow for cross application
    /// producer consumer flows.
    /// </summary>
    /// <param name="services">The service collection to enrich.</param>
    /// <param name="configuration">The application configuration to digest.</param>
    /// <returns>The enriched service collection.</returns>
    public static IServiceCollection AddAWS(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AWSOptions>()
            .BindConfiguration("Aws")
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AWSOptions>>().Value;

            return new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey);
        });

        services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AWSOptions>>().Value;

            var credentials = sp.GetRequiredService<BasicAWSCredentials>();

            return new AmazonSimpleNotificationServiceClient(
                credentials,
                new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = options.Url
                });
        });

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AWSOptions>>().Value;

            var credentials = sp.GetRequiredService<BasicAWSCredentials>();

            return new AmazonSQSClient(
                credentials,
                new AmazonSQSConfig
                {
                    ServiceURL = options.Url
                });
        });

        return services;
    }

    /// <summary>
    /// Adds a new SQS consumer.
    /// </summary>
    /// <typeparam name="TConsumer">The application's new consumer.</typeparam>
    /// <typeparam name="TQueueDescriptor">The queue registration which will drive the consumer.</typeparam>
    /// <typeparam name="TPayload">The type of the messages the consumer will accept.</typeparam>
    /// <param name="services">The service collection to enrich.</param>
    /// <returns>The enriched service collection.</returns>
    public static IServiceCollection AddConsumer<TConsumer, TQueueDescriptor, TPayload>(this IServiceCollection services)
        where TQueueDescriptor : class, IQueueDescriptor
        where TConsumer : SQSConsumer<TPayload, TQueueDescriptor>
        where TPayload : class
    {
        services.AddSingleton<TQueueDescriptor>();

        services.AddSingleton<IQueueFactory<TQueueDescriptor>, QueueFactory<TQueueDescriptor>>();

        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IQueueFactory<TQueueDescriptor>>();

            return factory.Create().GetAwaiter().GetResult();
        });

        services.AddSingleton(sp =>
        {
            var queueDetails = sp.GetRequiredService<QueueDetails<TQueueDescriptor>>();
            var sqs = sp.GetRequiredService<IAmazonSQS>();
            var options = sp.GetRequiredService<IOptions<AWSOptions>>();
            var logger = sp.GetRequiredService<ILogger<SQSSource<TQueueDescriptor>>>();

            return new SQSSource<TQueueDescriptor>(queueDetails, sqs, options, logger);
        });

        services.AddHostedService<TConsumer>();

        return services;
    }

    /// <summary>
    /// Add a new SNS producer.
    /// </summary>
    /// <typeparam name="TPayload">The type of the messages the producer will create and send.</typeparam>
    /// <typeparam name="TProducer">The application's new producer.</typeparam>
    /// <typeparam name="TTopicDescriptor">Details the destination for the producer's output.</typeparam>
    /// <param name="services">The service collection to enrich.</param>
    /// <returns>The enriched service collection.</returns>
    public static IServiceCollection AddProducer<TPayload, TProducer, TTopicDescriptor>(this IServiceCollection services)
        where TPayload : class
        where TProducer : class, ISNSProducer<TPayload, TTopicDescriptor>
        where TTopicDescriptor : class, ITopicDescriptor
    {
        services.AddScoped<TTopicDescriptor>();
        services.AddScoped<ISNSProducer<TPayload, TTopicDescriptor>, TProducer>();

        return services;
    }
}
