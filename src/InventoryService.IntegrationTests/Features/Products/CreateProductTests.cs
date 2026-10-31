using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace InventoryService.IntegrationTests.Features.Products;

public class CreateProductTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/product";

    public CreateProductTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsOk(CreateProductIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Read<CreateProductResponse>();

        result.Should().NotBeNull();

        result.ProductId.Should().BeGreaterThan(0);

        response.AssertHeader("location", $"{RequestURI}?productId={result.ProductId}");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsBadRequest(CreateProductIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        request.ProductName = string.Empty;

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("Payload.ProductName", "'Product Name' must not be empty.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsUnauthorized(CreateProductIntRequest request)
    {
        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsBadRequest_WhenProductAlreadyExists(CreateProductIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        request.ProductName = "Hammer";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("ProductName", "A Product with the requested Product Name already exists.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsBadRequest_WhenCountryDoesNotExist(CreateProductIntRequest request)
    {
        // Arrange
        AddBasicAuthHeader();

        request.ProductCountries!.First().CountryId = "300";

        // Act
        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("CountryId", "One or more of the requested Countries does not exist.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsInternalServerError(
        CreateProductIntRequest request,
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
