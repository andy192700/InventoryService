namespace InventoryService.Features.User.WhoAmI;

/// <summary>
/// The WhoAmI response, containing the authetnicated user's name and roles.
/// </summary>
public class WhoAmIResponse
{
    /// <summary>
    /// The authenticated user's username.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// The authenticated user's roles.
    /// </summary>
    public List<string> Roles { get; set; } = [];
}
