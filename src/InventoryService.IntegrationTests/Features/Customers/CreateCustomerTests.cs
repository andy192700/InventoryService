using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using static InventoryService.IntegrationTests.Features.Customers.CustomerRequests;

namespace InventoryService.IntegrationTests.Features.Customers;

public class CreateCustomerTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/customer";

    public CreateCustomerTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateCustomer_ReturnsOk(CreateCustomerIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Read<CreateCustomerResponse>();

        result.Should().NotBeNull();

        result.CustomerId.Should().BeGreaterThan(0);

        response.AssertHeader("location", $"{RequestURI}?customerId={result.CustomerId}");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateCustomer_ReturnsBadRequest(CreateCustomerIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();
        request.FirstName = string.Empty;

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("Payload.FirstName", "'First Name' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateCustomer_ReturnsUnauthorized(CreateCustomerIntRequest request)
    {
        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Missing Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateCustomer_ReturnsBadRequest_WhenCustomerNameAlreadyExists(CreateCustomerIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();
        request.FirstName = "John";
        request.LastName = "Smith";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("FirstName", "A Customer with the requested full name already exists.");
        result.AssertValidationErrorPresent("LastName", "A Customer with the requested full name already exists.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateCustomer_ReturnsInternalServerError(
        CreateCustomerIntRequest request,
        Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<ICreateCustomerDBCommand>();

            var repository = new Mock<ICreateCustomerDBCommand>();

            repository
                .Setup(x => x.CreateCustomerAsync(
                    It.IsAny<CreateCustomerDBParameters>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");
    }
}