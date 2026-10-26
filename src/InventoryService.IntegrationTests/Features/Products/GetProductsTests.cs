using FluentAssertions;
using InventoryService.Features.Products.GetProducts;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Products;

public class GetProductsTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/products";

    public GetProductsTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        // Act
        AddBasicAuthHeader();

        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetProductsResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);

        result.First().ProductId.Should().Be(1);
        result.First().CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.First().ProductName.Should().Be("Hammer");
        result.First().ProductDesc.Should().Be("16oz claw hammer");

        result.First().ProductCountries.Should().HaveCount(3);

        result.First().ProductCountries.First().CountryId.Should().Be(1);
        result.First().ProductCountries.First().UnitPrice.Should().Be(19.99);
    }

    [Fact]
    public async Task GetProducts_WithFilter_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var filter = "ha";

        // Act
        var response = await Client.GetAsync($"{RequestURI}?filter={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetProductsResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.Last().ProductId.Should().Be(3);
        result.Last().CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.Last().ProductName.Should().Be("Saw");
        result.Last().ProductDesc.Should().Be("Hand saw");

        result.Last().ProductCountries.Should().HaveCount(3);

        result.Last().ProductCountries.Last().CountryId.Should().Be(3);
        result.Last().ProductCountries.Last().UnitPrice.Should().Be(22.99);

        result.All(x => string.Concat(x.ProductName, x.ProductDesc).Contains(filter, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task GetProducts_WithFilter_ReturnsBadRequest()
    {
        // Arrange
        AddBasicAuthHeader();

        var filter = new string('A', 101);

        // Act
        var response = await Client.GetAsync($"{RequestURI}?filter={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails($"The length of 'Filter' must be 100 characters or fewer. You entered {filter.Length} characters.");
    }

    [Fact]
    public async Task GetProducts_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Missing Authorization Header");
    }
}
