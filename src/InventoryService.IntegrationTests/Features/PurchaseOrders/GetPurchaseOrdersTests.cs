using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

public class GetPurchaseOrdersTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/purchase-orders";

    public GetPurchaseOrdersTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetPurchaseOrders_ReturnsOk()
    {
        // Act
        AddBasicAuthHeader();

        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetPurchaseOrdersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);

        result.First().PurchaseOrderId.Should().Be(1);
        result.First().OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        result.First().CustomerId.Should().Be(1);
        result.First().CountryId.Should().Be(2);

        result.First().PurchaseOrderLines.Should().HaveCount(2);

        result.First().PurchaseOrderLines.First().PurchaseOrderLineId.Should().Be(1);
        result.First().PurchaseOrderLines.First().ProductId.Should().Be(1);
        result.First().PurchaseOrderLines.First().OrderQuantity.Should().Be(5);
        result.First().PurchaseOrderLines.First().RowNo.Should().Be(1);
    }

    [Fact]
    public async Task GetPurchaseOrders_WithProductId_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = 1;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetPurchaseOrdersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);

        result.First().PurchaseOrderId.Should().Be(1);
        result.First().OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        result.First().CustomerId.Should().Be(1);
        result.First().CountryId.Should().Be(2);

        result.First().PurchaseOrderLines.Should().HaveCount(1);

        result.First().PurchaseOrderLines.First().PurchaseOrderLineId.Should().Be(1);
        result.First().PurchaseOrderLines.First().ProductId.Should().Be(productId);
        result.First().PurchaseOrderLines.First().OrderQuantity.Should().Be(5);
        result.First().PurchaseOrderLines.First().RowNo.Should().Be(1);

        result.All(x => x.PurchaseOrderLines.All(x => x.ProductId == productId)).Should().BeTrue();
    }

    [Fact]
    public async Task GetPurchaseOrders_WithProductIdAndCountryId_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = 1;
        var countryId = 2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}&countryId={countryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetPurchaseOrdersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        result.First().PurchaseOrderId.Should().Be(1);
        result.First().OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        result.First().CustomerId.Should().Be(1);
        result.First().CountryId.Should().Be(countryId);

        result.First().PurchaseOrderLines.Should().HaveCount(1);

        result.First().PurchaseOrderLines.First().PurchaseOrderLineId.Should().Be(1);
        result.First().PurchaseOrderLines.First().ProductId.Should().Be(productId);
        result.First().PurchaseOrderLines.First().OrderQuantity.Should().Be(5);
        result.First().PurchaseOrderLines.First().RowNo.Should().Be(1);

        result.All(x => x.PurchaseOrderLines.All(x => x.ProductId == productId)).Should().BeTrue();
        result.All(x => x.CountryId == countryId).Should().BeTrue();
    }

    [Fact]
    public async Task GetPurchaseOrders_WithProductIdAndCountryIdAndCustomerId_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = 1;
        var countryId = 2;
        var customerId = 5;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}&countryId={countryId}&customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetPurchaseOrdersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        result.First().PurchaseOrderId.Should().Be(5);
        result.First().OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        result.First().CustomerId.Should().Be(customerId);
        result.First().CountryId.Should().Be(countryId);

        result.First().PurchaseOrderLines.Should().HaveCount(1);

        result.First().PurchaseOrderLines.First().PurchaseOrderLineId.Should().Be(9);
        result.First().PurchaseOrderLines.First().ProductId.Should().Be(productId);
        result.First().PurchaseOrderLines.First().OrderQuantity.Should().Be(7);
        result.First().PurchaseOrderLines.First().RowNo.Should().Be(1);

        result.All(x => x.PurchaseOrderLines.All(x => x.ProductId == productId)).Should().BeTrue();
        result.All(x => x.CountryId == countryId).Should().BeTrue();
        result.All(x => x.CustomerId == customerId).Should().BeTrue();
    }

    [Fact]
    public async Task GetPurchaseOrders_WithProductId_ReturnsBadRequest()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = -1;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails($"ProductId must be a positive integer.");
    }

    [Fact]
    public async Task GetPurchaseOrders_ReturnsUnauthorized()
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
