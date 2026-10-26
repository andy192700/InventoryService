namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Authentication constants class to house key contants in one place.
/// </summary>
public class AuthenticationConstants
{
    /// <summary>
    /// The applications Authorization Scheme.
    /// </summary>
    public const string AuthorizationScheme = "Basic";

    /// <summary>
    /// The applications expected Authorization header key.
    /// </summary>
    public const string AuthorizationHeader = "Authorization";

    /// <summary>
    /// The applications Authentication type.
    /// </summary>
    public const string AuthenticationType = "Basic";

    /// <summary>
    /// The name of the application authentication policy, required for access to all endpoints.
    /// </summary>
    public const string AuthorizationPolicy = "AccessPolicy";
}
