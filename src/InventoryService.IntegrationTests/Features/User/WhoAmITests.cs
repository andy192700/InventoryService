using FluentAssertions;
using InventoryService.Features.User.WhoAmI;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;

namespace InventoryService.IntegrationTests.Features.User;

public class WhoAmITests : IntegrationTestBase
{
    private const string RequestURI = "/v1/user/whoami";

    public WhoAmITests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task WhoAmI_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var result = await response.Read<ProblemDetails>();

        result.Should().NotBeNull();

        result.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task WhoAmI_ReturnsAdminUser()
    {
        // Arrange
        AddAdminBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<WhoAmIResponse>();

        result.Should().NotBeNull();

        result.UserName.Should().Be("admin");
        result.Roles.Should().HaveCount(2);
        result.Roles.Should().Contain(UserPermission.Read.ToString());
        result.Roles.Should().Contain(UserPermission.Write.ToString());
    }

    [Fact]
    public async Task WhoAmI_ReturnsReadOnlyUser()
    {
        // Arrange
        AddReadOnlyBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<WhoAmIResponse>();

        result.Should().NotBeNull();

        result.UserName.Should().Be("user");
        result.Roles.Should().HaveCount(1);
        result.Roles.Should().Contain(UserPermission.Read.ToString());
    }

    [Fact]
    public async Task WhoAmI_ReturnsNoneUser()
    {
        // Arrange
        AddNoPermissionBasicAuthHeader();

        // Act
        var response = await Client.GetAsync(RequestURI);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Read<WhoAmIResponse>();

        result.Should().NotBeNull();

        result.UserName.Should().Be("noneuser");
        result.Roles.Should().HaveCount(0);
    }
}
