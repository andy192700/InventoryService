using Microsoft.Data.SqlClient;

namespace InventoryService.Infrastructure;

public interface ISQLConnectionProvider
{
    Task<SqlConnection> GetSqlConnectionAsync(string connectionName, CancellationToken cancellationToken);
}

public class SQLConnectionProvider : ISQLConnectionProvider
{
    private readonly IConfiguration _configuration;

    public SQLConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
