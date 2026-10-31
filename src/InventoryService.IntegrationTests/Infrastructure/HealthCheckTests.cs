using Docker.DotNet.Models;
using FluentAssertions;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Health;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Infrastructure;

public class HealthCheckTests : IntegrationTestBase
{
    private const string RequestURI = "/health";

    public HealthCheckTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetHealth_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Arrange
        AddBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<HealthCheckResponse>();

        result.Should().NotBeNull();

        result.Status.Should().Be(HealthStatus.Healthy.ToString());
        result.Checks.Should().HaveCount(1);
        result.Checks.First().Key.Should().Be(nameof(DatabaseHealthCheck));
        result.Checks.First().Description.Should().Be("Service is healthy.");
        result.Checks.First().Value.Should().Be(HealthStatus.Healthy.ToString());
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task GetHealth_ReturnsUnhealthy(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<ISQLPrimativeActivity>();

            var repository = new Mock<ISQLPrimativeActivity>();

            repository
                .Setup(x => x.ConnectAsync(
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };
        AddBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<HealthCheckResponse>();

        result.Should().NotBeNull();

        result.Status.Should().Be(HealthStatus.Unhealthy.ToString());
        result.Checks.Should().HaveCount(1);
        result.Checks.First().Key.Should().Be(nameof(DatabaseHealthCheck));
        result.Checks.First().Description.Should().Be("Service is unhealthy.");
        result.Checks.First().Value.Should().Be(HealthStatus.Unhealthy.ToString());
    }
}
