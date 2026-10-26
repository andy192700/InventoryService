using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace InventoryService.IntegrationTests.Features.PurchaseOrders;

public class CreatePurchaseOrderTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/purchase-order";

    public CreatePurchaseOrderTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsOk(CreatePurchaseOrderIntRequest request)
    {
        // Act
        AddBasicAuthHeader();

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Read<CreatePurchaseOrderResponse>();

        result.Should().NotBeNull();

        result.PurchaseOrderId.Should().BeGreaterThan(0);

        response.AssertHeader("location", $"{RequestURI}?purchaseOrderId={result.PurchaseOrderId}");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsBadRequest(CreatePurchaseOrderIntRequest request)
    {
        // Act
        AddBasicAuthHeader();

        request.CustomerId = string.Empty;

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertProblemDetails($"CustomerId must be a positive integer.");
    }
}
