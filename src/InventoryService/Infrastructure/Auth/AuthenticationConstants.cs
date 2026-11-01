namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Authentication constants class to house key contants in one place.
/// </summary>
public class AuthenticationConstants
{
    /// <summary>
    /// The applications Authorization Scheme.
    /// </summary>
    public const string Scheme = "Basic";

    /// <summary>
    /// The applications expected Authorization header key.
    /// </summary>
    public const string AuthorizationHeader = "Authorization";

    /// <summary>
    /// The name of the application authorization policy for writing endpoints.
    /// </summary>
    public const string WritingAuthorizationPolicy = "Writing";

    /// <summary>
    /// The name of the application authorization policy for reading endpoints.
    /// </summary>
    public const string ReadingAuthorizationPolicy = "Reading";

    /// <summary>
    /// The name of the application authorization policy, requiring no roles.
    /// </summary>
    public const string NoAuthorizationPolicy = "None";
}
