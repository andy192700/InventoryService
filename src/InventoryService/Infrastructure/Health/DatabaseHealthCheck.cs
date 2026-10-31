using Dapper;
using InventoryService.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventoryService.Infrastructure.Health;

/// <summary>
/// A database healthcheck.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ISQLPrimativeActivity _activity;

    public DatabaseHealthCheck(ISQLPrimativeActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _activity.ConnectAndEstablishAsync("SELECT 1", cancellationToken);

            await _activity.CommenceAsync(async (cmd, connection) =>
            {
                return await connection.ExecuteScalarAsync<bool>(cmd);
            });

            return HealthCheckResult.Healthy("Service is healthy.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Service is unhealthy.");
        }
    }
}

/// <summary>
/// The <see cref="DatabaseHealthCheck"/>'s options, pre-built for ease of use.
/// </summary>
public class DatabaseHealthCheckOptions : HealthCheckOptions
{
    public DatabaseHealthCheckOptions() : base()
    {
        Predicate = check => check.Tags.Contains(nameof(DatabaseHealthCheck));
        ResponseWriter = HealthCheckResponseWriter.WriteHealthResponse();
    }

}