using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
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
        // Arrange
        AddBasicAuthHeader();

        // Act
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
        // Arrange
        AddBasicAuthHeader();

        request.CustomerId = string.Empty;

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("Payload.CustomerId", $"CustomerId must be a positive integer.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsUnauthorized(CreatePurchaseOrderIntRequest request)
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
    public async Task CreatePurchaseOrder_ReturnsBadRequest_WhenCustomerDoesNotExist(CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        request.CustomerId = "300";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CustomerId", $"A Customer with the requested CustomerId does not exist.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsBadRequest_WhenProductIdDoesNotMatchSuppliedCountry(CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        request.lines!.First().ProductId = "300";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CountryId", "One or more of the requested Product Countries does not exist.");
        result.AssertValidationErrorPresent("ProductId", "One or more of the requested Product Countries does not exist.");
    }


    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsInternalServerError(
        CreatePurchaseOrderIntRequest request,
        Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<ISQLTransactionActivity>();

            var repository = new Mock<ISQLTransactionActivity>();

            repository
                .Setup(x => x.ConnectAsync(
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
