using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;

namespace InventoryService.Extensions;

public static class SQLActivityExtensions
{
    /// <summary>
    /// Grants a <see cref="ISQLTransactionActivity"/> facade the capability to digest and act upon a <see cref="SqlException"/>,
    /// should it manifest.
    /// </summary>
    /// <param name="activity">The database facade.</param>
    /// <param name="OnSqlException">Supplied <see cref="Action"/> ingesting the <see cref="SqlException"/>.</param>
    /// <returns>A task of the facade type in which the operation will complete.</returns>
    public static async Task<ISQLActivity> WhenCapturingSqlException(
        this ISQLTransactionActivity activity,
        Action<SqlException> OnSqlException)
    {
        activity.WhenCapturingSqlException(OnSqlException);

        return activity;
    }

    /// <summary>
    /// Calls a database facade's connect and command preparation in one hit, reusing the cancellation token.
    /// </summary>
    /// <param name="activity">The database facade.</param>
    /// <param name="command">The store procedure to be executed which should include the database schema.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <param name="parameters">Optional anonymous parameters supplied to the stored procedure.</param>
    /// <returns>A task of the facade type in which the operation will complete.</returns>
    public static async Task<ISQLActivity> ConnectAndEstablishAsync(
        this ISQLActivity activity,
        string command,
        CancellationToken cancellationToken,
        object? parameters = default)
    {
        await activity.ConnectAsync(cancellationToken);

        await activity.EstablishAsync(command, cancellationToken, parameters);

        return activity;
    }
}