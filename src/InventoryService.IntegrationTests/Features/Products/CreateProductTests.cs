using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;
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
        // Act
        AddBasicAuthHeader();

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
        // Act
        AddBasicAuthHeader();

        request.ProductName = string.Empty;

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("'Product Name' must not be empty.");
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

        result.Detail.Should().Be("Missing Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsBadRequest_WhenProductAlreadyExists(CreateProductIntRequest request)
    {
        // Act
        AddBasicAuthHeader();

        request.ProductName = "Hammer";

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("A Product with the requested Product Name already exists.");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task CreateProduct_ReturnsBadRequest_WhenCountryDoesNotExist(CreateProductIntRequest request)
    {
        // Act
        AddBasicAuthHeader();

        request.ProductCountries!.First().CountryId = "300";

        var response = await Client.PostAsJsonAsync(RequestURI, request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("One or more of the requested Countries does not exist.");
    }
}
