using Microsoft.Data.SqlClient;

namespace InventoryService.Infrastructure;

/// <summary>
/// Provides a fresh SQL connection.
/// </summary>
public interface ISQLConnectionProvider
{
    /// <summary>
    /// Create and open the SQL connection.
    /// </summary>
    /// <param name="connectionName">The name of the connection, as per loaded configuration.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An asynchronous SQL Connection, which upon being returned successfully, will be open.</returns>
    Task<SqlConnection> GetSqlConnectionAsync(string connectionName, CancellationToken cancellationToken);
}

/// <summary>
/// Provides a fresh SQL connection.
/// </summary>
public class SQLConnectionProvider : ISQLConnectionProvider
{
    private readonly IConfiguration _configuration;

    public SQLConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<SqlConnection> GetSqlConnectionAsync(string connectionName, CancellationToken cancellationToken)
    {
        var connString = _configuration.GetConnectionString(connectionName);

        if (string.IsNullOrWhiteSpace(connString))
        {
            throw new InvalidOperationException($"Connection string '{connectionName}' was not found.");
        }

        var conn = new SqlConnection(connString);

        await conn.OpenAsync(cancellationToken);

        return conn;
    }
}
