using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;
using Microsoft.AspNetCore.Mvc;
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
        // Act
        AddBasicAuthHeader();

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
        // Act
        AddBasicAuthHeader();
        request.FirstName = string.Empty;

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("'First Name' must not be empty.");
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
        // Act
        AddBasicAuthHeader();
        request.FirstName = "John";
        request.LastName = "Smith";

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("A Customer with the requested full name already exists.");
    }
}