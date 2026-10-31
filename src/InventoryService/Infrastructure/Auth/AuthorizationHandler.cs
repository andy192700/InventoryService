using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Specifies the applications required properties for authorization.
/// </summary>
public class AuthorizationHandlerRequirement : IAuthorizationRequirement
{
    public string RequiredAuthenticationType { get; } = AuthenticationConstants.Scheme;

    public UserPermission RequiredRole { get; private set; }

    public AuthorizationHandlerRequirement(UserPermission requiredRole)
    {
        RequiredRole = requiredRole;
    }
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

        if (identity != null
            && identity.IsAuthenticated
            && identity.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == requirement.RequiredRole.ToString())
            && identity.AuthenticationType == requirement.RequiredAuthenticationType)
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
