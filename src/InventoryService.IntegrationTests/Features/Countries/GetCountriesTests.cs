using FluentAssertions;
using InventoryService.Features.Countries.GetCountries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Countries;

public class GetCountriesTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/countries";

    public GetCountriesTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetCountries_ReturnsOk()
    {
        // Arrange
        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetCountriesResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        result.First().CountryId.Should().Be(1);
        result.First().CountryCode.Should().Be("US");
        result.First().CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public async Task GetCountries_ReturnsOk_WhenReadOnlyUserCalls()
    {
        // Arrange
        AddReadOnlyBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetCountriesResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        result.First().CountryId.Should().Be(1);
        result.First().CountryCode.Should().Be("US");
        result.First().CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public async Task GetCountries_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task GetCountries_ReturnsInternalServerError(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<IGetCountriesDBQuery>();

            var repository = new Mock<IGetCountriesDBQuery>();

            repository
                .Setup(x => x.GetCountriesAsync(
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            services.AddScoped(_ => repository.Object);
        };

        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Internal Server Error");
    }
}