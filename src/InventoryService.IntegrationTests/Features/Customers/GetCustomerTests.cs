using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Customers;

public class GetCustomerTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/customer";

    public GetCustomerTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetCustomer_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var customerId = 3;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<GetCustomerResponse>();

        result.Should().NotBeNull();

        result.CustomerId.Should().Be(customerId);
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.FirstName.Should().Be("Pierre");
        result.LastName.Should().Be("Dubois");
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound()
    {
        // Arrange
        AddBasicAuthHeader();

        var customerId = 300;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Customer not found");
    }

    [Fact]
    public async Task GetCustomer_ReturnsBadRequest()
    {
        // Arrange
        AddBasicAuthHeader();
        var customerId = -3;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CustomerId", $"CustomerId must be a positive integer.");
    }

    [Fact]
    public async Task GetCustomer_ReturnsUnauthorized()
    {
        // Arrange
        var customerId = 3;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Missing Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task GetCustomer_ReturnsInternalServerError(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<ISQLActivity>();

            var repository = new Mock<ISQLActivity>();

            repository
                .Setup(x => x.ConnectAsync(
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddBasicAuthHeader();

        var customerId = 3;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");
    }
}
