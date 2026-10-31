using Dapper;
using InventoryService.Extensions;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Contract for obtaining user permissions.
/// </summary>
public interface IGetUserPermissionsDBQuery
{
    /// <summary>
    /// Queries the database, obtaining the user's permissions.
    /// </summary>
    /// <param name="userName">The supplied user name.</param>
    /// <param name="passwordHash">The supplied hashed password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The user permissions.</returns>
    Task<UserPermissions> GetPermissionsAsync(string userName, string passwordHash, CancellationToken cancellationToken);
}

/// <summary>
/// Concrete implementation for obtaining user credentials.
/// </summary>
public class GetUserPermissionsDBQuery : IGetUserPermissionsDBQuery
{
    private readonly ISQLActivity _activity;

    public GetUserPermissionsDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<UserPermissions> GetPermissionsAsync(string userName, string passwordHash, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "governance.usp_GetCredentials",
            cancellationToken,
            new
            {
                UserName = userName,
                PasswordHash = passwordHash
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            var userName = await result.ReadFirstOrDefaultAsync<string>();

            var permissions = (await result.ReadAsync<string>()).ToList();

            return new UserPermissions
            {
                UserName = userName,
                Permissions = permissions
            };
        });
    }
}