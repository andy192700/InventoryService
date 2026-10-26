using Dapper;
using System.Data;

namespace InventoryService.Infrastructure;

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
    private readonly ISQLConnectionProvider _connectionProvider;

    public VerifyUserQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    /// <inheritdoc/>
    public async Task<bool> VerifyUserAsync(string userName, string passwordHash, CancellationToken cancellationToken)
    {
        var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        return conn.ExecuteScalar<bool>(
            "dbo.usp_VerifyCredentials",
            param: new
            {
                UserName = userName,
                PasswordHash = passwordHash
            },
            commandType: CommandType.StoredProcedure);
    }
}