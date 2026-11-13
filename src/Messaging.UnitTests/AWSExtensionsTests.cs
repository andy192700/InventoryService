using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Messaging.UnitTests;

public class AWSExtensionsTests
{

    [Theory]
    [InlineAutoMoqData]
    public void AddAWS_BindsConfigurationToOptions(
        string accessKey,
        string secretKey)
    {
        // Arrange
        var url = "http://127.0.0.1";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Aws:AccessKey"] = accessKey,
                ["Aws:SecretKey"] = secretKey,
                ["Aws:Url"] = url
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddSingleton<IConfiguration>(sp => configuration);
        services.AddAWS(configuration);

        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AWSOptions>>();

        // Assert
        options.Value.AccessKey.Should().Be(accessKey);
        options.Value.SecretKey.Should().Be(secretKey);
        options.Value.Url.Should().Be(url);

        provider.GetService<BasicAWSCredentials>()
            .Should()
            .NotBeNull();

        provider.GetService<IAmazonSimpleNotificationService>()
            .Should()
            .NotBeNull();

        provider.GetService<IAmazonSQS>()
            .Should()
            .NotBeNull();

        provider.GetService<IOptions<AWSOptions>>()
            .Should()
            .NotBeNull();
    }

    [Theory]
    [InlineAutoMoqData]
    public void AddProducer_RegistersProducer(
        [Frozen] Mock<IAmazonSimpleNotificationService> mockSNS,
        string accessKey,
        string secretKey,
        string url)
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Aws:AccessKey"] = accessKey,
                ["Aws:SecretKey"] = secretKey,
                ["Aws:Url"] = url
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddSingleton<IConfiguration>(sp => configuration);
        services.AddAWS(configuration);
        services.AddSingleton(sp => mockSNS.Object);
        services.AddProducer<TestPayload, TestTopicDescriptor>();

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<TestTopicDescriptor>()
            .Should()
            .NotBeNull();

        provider.GetService<ISNSProducer<TestPayload, TestTopicDescriptor>>()
            .Should()
            .NotBeNull();

        services.Should().Contain(x =>
            x.ServiceType == typeof(TestTopicDescriptor) &&
            x.Lifetime == ServiceLifetime.Scoped);

        services.Should().Contain(x =>
            x.ServiceType == typeof(ISNSProducer<TestPayload, TestTopicDescriptor>) &&
            x.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddConsumer_RegistersConsumer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddConsumer<TestConsumer, TestQueueDescriptor>();

        // Assert
        services.Should().Contain(x =>
            x.ServiceType == typeof(TestQueueDescriptor) &&
            x.Lifetime == ServiceLifetime.Singleton);
        services.Should().Contain(x =>
            x.ServiceType == typeof(IQueueFactory<TestQueueDescriptor>) &&
            x.ImplementationType == typeof(QueueFactory<TestQueueDescriptor>) &&
            x.Lifetime == ServiceLifetime.Singleton);
        services.Should().Contain(x =>
            x.ServiceType == typeof(QueueDetails<TestQueueDescriptor>) &&
            x.Lifetime == ServiceLifetime.Singleton);
        services.Should().Contain(x =>
            x.ServiceType == typeof(ISQSReader<TestQueueDescriptor>) &&
            x.ImplementationType == typeof(SQSReader<TestQueueDescriptor>));
        services.Should().Contain(x =>
            x.ServiceType == typeof(ISQSConsumer) &&
            x.Lifetime == ServiceLifetime.Singleton &&
            x.ImplementationType == typeof(TestConsumer));
        services.Should().Contain(x =>
            x.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService));
    }

    private sealed class TestPayload;

    private sealed class TestTopicDescriptor : ITopicDescriptor
    {
        public string TopicName => "test-topic";
    }

    private sealed class TestQueueDescriptor : IQueueDescriptor
    {
        public string QueueName => "test-queue";

        public string TopicName => "test-queue";
    }

    private sealed class TestConsumer : SQSConsumer<TestPayload>
    {
        protected override Task ConsumeAsync(TestPayload payload, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}