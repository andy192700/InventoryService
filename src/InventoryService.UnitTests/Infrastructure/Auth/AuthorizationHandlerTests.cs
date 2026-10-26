using FluentAssertions;
using InventoryService.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventoryService.UnitTests.Infrastructure.Auth;

public class AuthorizationHandlerTests
{
    [Theory]
    [InlineAutoMoqData("Basic")]
    public async Task HandleRequirementAsync_Succeeds_WhenAuthenticationTypeMatches(
        string authType,
        AuthorizationHandlerRequirement requirement)
    {
        // Arrange
        var identity = new ClaimsIdentity(authenticationType: authType);
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext([requirement], principal, null);

        var sut = new AuthorizationHandler();

        // Act
        await sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        context.HasFailed.Should().BeFalse();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task HandleRequirementAsync_Fails_WhenAuthenticationTypeDoesNotMatch(
        string identityAuthType,
        AuthorizationHandlerRequirement requirement)
    {
        // Arrange
        var identity = new ClaimsIdentity(authenticationType: identityAuthType);
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext([requirement], principal, null);

        var sut = new AuthorizationHandler();

        // Act
        await sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();

        context.FailureReasons.Should().Contain(f => f.Message == "User is not authorized.");
    }
}