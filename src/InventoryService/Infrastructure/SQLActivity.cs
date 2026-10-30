using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace InventoryService.Infrastructure;

/// <summary>
/// Facade for controlling database access.
/// </summary>
public interface ISQLActivity
{
    /// <summary>
    /// Within the facade, open one time usage database collection.
    /// </summary>
    /// <param name="cancellationToken">A Cancellation token.</param>
    /// <returns>A task in which the operation will complete.</returns>
    Task ConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Establish a command definition object within the facade, ready for use.
    /// </summary>
    /// <param name="command">The SQL command string, which must be a stored procedure (including it's schema).</param>
    /// <param name="cancellationToken">A Cancellation token.</param>
    /// <param name="parameters">Optional anonymous parameters supplied to the stored procedure.</param>
    /// <returns>A task in which the operation will complete.</returns>
    Task EstablishAsync(
        string command,
        CancellationToken cancellationToken,
        object? parameters = default);

    /// <summary>
    /// Using the facade's built up SQL Connection and Command Definition, run an asynchronous operation, calling the supplied stored procedure.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="operation">The delegate to perform the operation.</param>
    /// <returns>The desired output for the given requested type.</returns>
    Task<TOut> CommenceAsync<TOut>(Func<CommandDefinition, DbConnection, Task<TOut>> operation);
}

/// <summary>
/// Facade for controlling database access, when database transactions are required.
/// </summary>
public interface ISQLTransactionActivity : ISQLActivity
{
    /// <summary>
    /// Specify an action which allows the capture and per basis usage of a SQL Exception, should one arise.
    /// </summary>
    /// <param name="onSqlException"></param>
    void WhenCapturingSqlException(Action<SqlException> onSqlException);
}

/// <summary>
/// Abstract basis for facade for managing database access.
/// </summary>
public abstract class SQLActivityBase : IAsyncDisposable
{
    private const string DatabaseName = "Inventory";

    private readonly ISQLConnectionProvider _connectionProvider;

    private DbConnection? _connection;

    /// <summary>
    /// This operation's active database connection.
    /// </summary>
    protected DbConnection Connection
    {
        get
        {
            if (_connection is null)
            {
                throw new InvalidOperationException("Unable to establish a sql command without first creating a connection.");
            }

            return _connection;
        }
        set
        {
            _connection = value;
        }
    }

    private CommandDefinition? _cmd;

    /// <summary>
    /// This operation's prepared <see cref="CommandDefinition"/>.
    /// </summary>
    protected CommandDefinition Cmd
    {
        get
        {
            if (_cmd is null)
            {
                throw new InvalidOperationException("Unable to perform a sql operation as the command has not been created.");
            }

            return _cmd.Value;
        }
        set
        {
            _cmd = value;
        }
    }

    public SQLActivityBase(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task ConnectAsync(
        CancellationToken cancellationToken)
    {
        _connection = await _connectionProvider.GetSqlConnectionAsync(DatabaseName, cancellationToken);
    }

    /// <inheritdoc/>
    public abstract Task EstablishAsync(
        string command,
        CancellationToken cancellationToken,
        object? parameters = default);

    /// <inheritdoc/>
    public abstract Task<TOut> CommenceAsync<TOut>(Func<CommandDefinition, DbConnection, Task<TOut>> operation);

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Concrete facade for managing database acivity using transactions and exception management (useful when performing writes).
/// </summary>
public sealed class SQLTransactionActivity : SQLActivityBase, ISQLTransactionActivity, IAsyncDisposable
{
    private DbTransaction? _transaction;

    /// <summary>
    /// This operation's SQL Transaction.
    /// </summary>
    private DbTransaction Transaction
    {
        get
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("This operation requires a Sql transaction to encapsulate its invocation.");
            }

            return _transaction;
        }
        set
        {
            _transaction = value;
        }
    }

    private Action<SqlException>? _onSqlException;

    /// <summary>
    /// Invoked when a <see cref="SqlException"/> manifests.
    /// </summary>
    private Action<SqlException> OnSqlException
    {
        get
        {
            if (_onSqlException is null)
            {
                return _ => { };
            }

            return _onSqlException;
        }
        set
        {
            _onSqlException = value;
        }
    }

    public SQLTransactionActivity(ISQLConnectionProvider connectionProvider) : base(connectionProvider) { }

    /// <inheritdoc/>
    public override async Task EstablishAsync(
        string command,
        CancellationToken cancellationToken,
        object? parameters = default)
    {
        _transaction = await Connection.BeginTransactionAsync(cancellationToken);

        Cmd = new CommandDefinition(
            command,
            parameters: parameters,
            transaction: _transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public void WhenCapturingSqlException(Action<SqlException> onSqlException)
        => OnSqlException = onSqlException;

    /// <inheritdoc/>
    public override async Task<TOut> CommenceAsync<TOut>(Func<CommandDefinition, DbConnection, Task<TOut>> operation)
    {
        TOut result;

        try
        {
            result = await operation(Cmd, Connection);

            await Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                OnSqlException.Invoke(sqlEx);

                await Transaction.RollbackAsync();
            }

            throw;
        }

        return result;
    }

    /// <inheritdoc/>
    public new async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
        }

        await base.DisposeAsync();
    }
}

/// <summary>
/// Concrete for facade for managing database contact.
/// </summary>
public sealed class SQLActivity : SQLActivityBase, ISQLActivity, IAsyncDisposable
{
    public SQLActivity(ISQLConnectionProvider connectionProvider) : base(connectionProvider) { }

    /// <inheritdoc/>
    public override async Task<ISQLActivity> EstablishAsync(
        string command,
        CancellationToken cancellationToken,
        object? parameters = default)
    {
        Cmd = new CommandDefinition(
            command,
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return this;
    }

    /// <inheritdoc/>
    public override async Task<TOut> CommenceAsync<TOut>(Func<CommandDefinition, DbConnection, Task<TOut>> operation)
    {
        return await operation(Cmd, Connection);
    }

    /// <inheritdoc/>
    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}
