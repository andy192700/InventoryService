using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using Messaging;
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
    public async Task CreatePurchaseOrder_ReturnsOk(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer, 
        CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Read<CreatePurchaseOrderResponse>();

        result.Should().NotBeNull();

        result.PurchaseOrderId.Should().BeGreaterThan(0);

        response.AssertHeader("location", $"{RequestURI}/{result.PurchaseOrderId}");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsBadRequest(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer, 
        CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        AddAdminBasicAuthHeader();

        request.CustomerId = string.Empty;

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("Payload.CustomerId", $"CustomerId must be a positive integer.");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsUnauthorized(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer, 
        CreatePurchaseOrderIntRequest request)
    {
        // Act
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsForbidden_WhenReadOnlyUserCalls(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer, 
        CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        AddReadOnlyBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Forbidden");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsBadRequest_WhenCustomerDoesNotExist(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer, 
        CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        AddAdminBasicAuthHeader();

        request.CustomerId = "300";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CustomerId", $"A Customer with the requested CustomerId does not exist.");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsBadRequest_WhenProductIdDoesNotMatchSuppliedCountry(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer,
        CreatePurchaseOrderIntRequest request)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
        };

        AddAdminBasicAuthHeader();

        request.lines!.First().ProductId = "300";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CountryId", "One or more of the requested Product Countries does not exist.");
        result.AssertValidationErrorPresent("ProductId", "One or more of the requested Product Countries does not exist.");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreatePurchaseOrder_ReturnsInternalServerError(
        [Frozen] Mock<ISNSProducer<CreatePurchaseOrderResponse, PurchaseOrderCreatedTopic>> mockProducer,
        CreatePurchaseOrderIntRequest request,
        Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.AddScoped(x => mockProducer.Object);
            services.RemoveAll<ICreatePurchaseOrderDBCommand>();

            var repository = new Mock<ICreatePurchaseOrderDBCommand>();

            repository
                .Setup(x => x.CreatePurchaseOrderAsync(
                    It.IsAny<CreatePurchaseOrderDBParameters>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");

        mockProducer.Verify(x => x.ProduceAsync(It.IsAny<CreatePurchaseOrderResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
