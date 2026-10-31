namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Contains details regarding a user.
/// </summary>
public class UserPermissions
{
    /// <summary>
    /// The user's username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The user's permissions.
    /// </summary>
    public ICollection<string> Permissions { get; set; } = [];
}

/// <summary>
/// Enum containing all available user permissions, available at compile time.
/// </summary>
public enum UserPermission
{
    Write,
    Read
}