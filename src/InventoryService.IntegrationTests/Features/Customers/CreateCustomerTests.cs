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
        request.FirstName = string.Empty;

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails("'First Name' must not be empty.");

    }
}