using Dapper;
using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

/// <summary>
/// Executes database commands to create a product.
/// </summary>
public interface ICreateProductDBCommand
{
    /// <summary>
    /// Creates a product in the database.
    /// </summary>
    /// <param name="parameters">The database parameters for the command.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A database response containing the created product.</returns>
    Task<CreateProductDBResponse> CreateProductAsync(
        CreateProductDBParameters parameters,
        CancellationToken cancellationToken);
}

/// <summary>
/// Executes database commands to create a product.
/// </summary>
public class CreateProductDBCommand : ICreateProductDBCommand
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    private readonly ILogger<CreateProductDBCommand> _logger;

    public CreateProductDBCommand(
        ISQLConnectionProvider sqlConnectionProvider,
        ILogger<CreateProductDBCommand> logger)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CreateProductDBResponse> CreateProductAsync(CreateProductDBParameters parameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var transaction = await conn.BeginTransactionAsync();

        int productId;

        try
        {
            var cmd = new CommandDefinition(
                "warehouse.usp_CreateProduct",
                transaction: transaction,
                parameters: new
                {
                    parameters.ProductName,
                    parameters.ProductDesc,
                    parameters.ProductCountries
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            productId = await conn.ExecuteScalarAsync<int>(cmd);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                _logger.LogError("Failed to create purchase order");

                await transaction.RollbackAsync();
            }

            throw;
        }

        return new CreateProductDBResponse { ProductId = productId };
    }
}