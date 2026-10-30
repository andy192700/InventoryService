using Dapper;
using InventoryService.Extensions;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Contract for verifying user credentials.
/// </summary>
public interface IVerifyUserQuery
{
    /// <summary>
    /// Queries the database, verifying the user credentials are valid
    /// </summary>
    /// <param name="userName">The supplied user name.</param>
    /// <param name="passwordHash">The supplied hashed password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns></returns>
    Task<bool> VerifyUserAsync(string userName, string passwordHash, CancellationToken cancellationToken);
}

/// <summary>
/// Concrete implementation for verifying user credentials.
/// </summary>
public class VerifyUserQuery : IVerifyUserQuery
{
    private readonly ISQLActivity _activity;

    public VerifyUserQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<bool> VerifyUserAsync(string userName, string passwordHash, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "governance.usp_VerifyCredentials",
            cancellationToken,
            new
            {
                UserName = userName,
                PasswordHash = passwordHash
            });

        return await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<bool>(cmd);
        });
    }
}