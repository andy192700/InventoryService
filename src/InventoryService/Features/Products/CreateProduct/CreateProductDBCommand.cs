using Dapper;
using InventoryService.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryService.Features.Products.CreateProduct;

public interface ICreateProductDBCommand
{
    Task<CreateProductDBResponse> CreateProductAsync(
        CreateProductDBParameters parameters,
        CancellationToken cancellationToken);
}

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

    public async Task<CreateProductDBResponse> CreateProductAsync(CreateProductDBParameters parameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var transaction = await conn.BeginTransactionAsync();

        int productId;

        try
        {
            productId = await conn.ExecuteScalarAsync<int>(
                "dbo.usp_CreateProduct",
                transaction: transaction,
                param: new
                {
                    parameters.ProductName,
                    parameters.ProductDesc,
                    parameters.ProductCountries
                },
                commandType: CommandType.StoredProcedure);

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
