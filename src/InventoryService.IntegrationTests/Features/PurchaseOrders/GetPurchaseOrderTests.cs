using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

public class GetPurchaseOrderTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/purchase-order";

    public GetPurchaseOrderTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var purchaseOrderId = 2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<GetPurchaseOrderResponse>();

        result.Should().NotBeNull();

        result.PurchaseOrderId.Should().Be(purchaseOrderId);
        result.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        result.CustomerId.Should().Be(2);
        result.CountryId.Should().Be(2);

        result.PurchaseOrderLines.Should().HaveCount(2);

        result.PurchaseOrderLines.First().PurchaseOrderLineId.Should().Be(3);
        result.PurchaseOrderLines.First().ProductId.Should().Be(1);
        result.PurchaseOrderLines.First().OrderQuantity.Should().Be(3);
        result.PurchaseOrderLines.First().RowNo.Should().Be(1);
    }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsNotFound()
    {
        // Arrange
        AddBasicAuthHeader();

        var purchaseOrderId = 200;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Purchase Order not found");
    }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsBadRequest()
    {
        // Arrange
        AddBasicAuthHeader();

        var purchaseOrderId = -2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails($"PurchaseOrderId must be a positive integer.");
    }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsUnauthorized()
    {
        // Act
        var purchaseOrderId = 2;

        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Missing Authorization Header");
    }
}
