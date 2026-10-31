using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Specifies the applications required properties for authorization.
/// </summary>
public class AuthorizationHandlerRequirement : IAuthorizationRequirement
{
    public string RequiredAuthenticationType { get; } = AuthenticationConstants.Scheme;
}

/// <summary>
/// Handles the authorization process for an authenticated user.
/// </summary>
public class AuthorizationHandler : AuthorizationHandler<AuthorizationHandlerRequirement>
{
    /// <inheritdoc/>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationHandlerRequirement requirement)
    {
        var identity = context.User.Identity as ClaimsIdentity;

        if (identity != null &&
            identity.AuthenticationType == requirement.RequiredAuthenticationType)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not authorized."));
        }

        return Task.CompletedTask;
    }
}
