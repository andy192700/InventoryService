using FluentAssertions;
using InventoryService.Features.Products.GetProduct;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Products;

public class GetProductTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/product";

    public GetProductTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetProduct_ReturnsOk()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = 4;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<GetProductResponse>();

        result.Should().NotBeNull();

        result.ProductId.Should().Be(productId);
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.ProductName.Should().Be("Tape Measure");
        result.ProductDesc.Should().Be("5m tape measure");

        result.ProductCountries.Should().HaveCount(3);

        result.ProductCountries.First().CountryId.Should().Be(1);
        result.ProductCountries.First().UnitPrice.Should().Be(9.99);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = 400;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Product not found");
    }

    [Fact]
    public async Task GetProduct_ReturnsBadRequest()
    {
        // Arrange
        AddBasicAuthHeader();

        var productId = -4;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("ProductId", $"ProductId must be a positive integer.");
    }

    [Fact]
    public async Task GetProduct_ReturnsUnauthorized()
    {
        // Arrange
        var productId = 4;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Detail.Should().Be("Missing Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task GetProduct_ReturnsInternalServerError(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<ISQLActivity>();

            var repository = new Mock<ISQLActivity>();

            repository
                .Setup(x => x.ConnectAsync(
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddBasicAuthHeader();

        var productId = 4;

        // Act
        var response = await Client.GetAsync($"{RequestURI}?productId={productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");
    }
}