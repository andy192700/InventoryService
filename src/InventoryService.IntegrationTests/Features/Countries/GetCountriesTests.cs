using FluentAssertions;
using InventoryService.Features.Countries.GetCountries;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Countries;

public class GetCountriesTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/countries";

    public GetCountriesTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetCountries_ReturnsOk()
    {
        // Act
        AddBasicAuthHeader();
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
}