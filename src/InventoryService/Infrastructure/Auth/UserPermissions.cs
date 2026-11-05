namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Contains details regarding a user.
/// </summary>
/// <param name="UserName">The user's username.</param>
/// <param name="Permissions">The user's permissions.</param>
public record UserPermissions
(
    string? UserName,
    ICollection<string> Permissions
);

/// <summary>
/// Enum containing all available user permissions, available at compile time.
/// </summary>
public enum UserPermission
{
    Write,
    Read
}