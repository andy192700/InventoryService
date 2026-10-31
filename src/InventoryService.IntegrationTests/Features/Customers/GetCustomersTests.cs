using FluentAssertions;
using InventoryService.Features.Customers.GetCustomers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Features.Customers;

public class GetCustomersTests : IntegrationTestBase
{
    private const string RequestURI = "/v1/inventory/customers";

    public GetCustomersTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetCustomers_ReturnsOk()
    {
        // Arrange
        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetCustomersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);

        result.First().CustomerId.Should().Be(1);
        result.First().CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.First().FirstName.Should().Be("John");
        result.First().LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task GetCustomers_ReturnsOk_WhenReadOnlyUserCalls()
    {
        // Arrange
        AddReadOnlyBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetCustomersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);

        result.First().CustomerId.Should().Be(1);
        result.First().CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.First().FirstName.Should().Be("John");
        result.First().LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task GetCustomers_WithFilter_ReturnsOk()
    {
        // Arrange
        AddAdminBasicAuthHeader();
        var filter = "jo";

        // Act
        var response = await Client.GetAsync($"{RequestURI}?filter={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<ICollection<GetCustomersResponse>>();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.Last().CustomerId.Should().Be(2);
        result.Last().CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(30));
        result.Last().FirstName.Should().Be("Jane");
        result.Last().LastName.Should().Be("Jones");

        result.All(x => string.Concat(x.FirstName, x.LastName).Contains(filter, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task GetCustomers_WithFilter_ReturnsBadRequest()
    {
        // Arrange
        AddAdminBasicAuthHeader();
        var filter = new string('A', 101);

        // Act
        var response = await Client.GetAsync($"{RequestURI}?filter={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.AssertValidationErrorPresent("Filter", $"The length of 'Filter' must be 100 characters or fewer. You entered {filter.Length} characters.");
    }

    [Fact]
    public async Task GetCustomers_ReturnsUnauthorized()
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
    public async Task GetCustomers_ReturnsInternalServerError(Exception exception)
    {
        // Arrange
        WebApplicationFactory.OverrideTestServices = services =>
        {
            services.RemoveAll<IGetCustomersDBQuery>();

            var repository = new Mock<IGetCustomersDBQuery>();

            repository
                .Setup(x => x.GetCustomersAsync(
                    It.IsAny<GetCustomersDBParameters>(),
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
