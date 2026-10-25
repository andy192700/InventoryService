using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Net.WebRequestMethods;

namespace InventoryService.IntegrationTests.Features.Customers;

public class GetCustomerTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/customer";

    public GetCustomerTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetCustomer_ReturnsOk()
    {
        // Arrange
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
        var customerId = -3;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails($"CustomerId must be a positive integer.");
    }
}
