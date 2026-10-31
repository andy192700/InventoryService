using FluentAssertions;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
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
        AddAdminBasicAuthHeader();

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
    public async Task GetPurchaseOrder_ReturnsOk_WhenReadOnlyUserCalls()
    {
        // Arrange
        AddReadOnlyBasicAuthHeader();

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
        AddAdminBasicAuthHeader();

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
        AddAdminBasicAuthHeader();

        var purchaseOrderId = -2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("PurchaseOrderId", $"PurchaseOrderId must be a positive integer.");
    }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsUnauthorized()
    {
        // Arrange
        var purchaseOrderId = 2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task GetPurchaseOrder_ReturnsForbidden()
    {
        // Arrange
        var purchaseOrderId = 2;

        AddNoPermissionBasicAuthHeader();

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Forbidden");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task GetPurchaseOrder_ReturnsInternalServerError(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<IGetPurchaseOrderDBQuery>();

            var repository = new Mock<IGetPurchaseOrderDBQuery>();

            repository
                .Setup(x => x.GetPurchaseOrderAsync(
                    It.IsAny<GetPurchaseOrderDBParameters>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddAdminBasicAuthHeader();

        var purchaseOrderId = 2;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?purchaseOrderId={purchaseOrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");
    }
}
