namespace InventoryService.Features.User.WhoAmI;

/// <summary>
/// The WhoAmI response, containing the authetnicated user's name and roles.
/// </summary>
/// <param name="UserName">The authenticated user's username.</param>
/// <param name="Roles">The authenticated user's roles.</param>
public record WhoAmIResponse
(
    string UserName,
    List<string> Roles
);
