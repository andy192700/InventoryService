using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

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
    private readonly ISQLTransactionActivity _activity;

    private readonly ILogger<CreateProductDBCommand> _logger;

    public CreateProductDBCommand(
        ISQLTransactionActivity activity,
        ILogger<CreateProductDBCommand> logger)
    {
        _activity = activity;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CreateProductDBResponse> CreateProductAsync(CreateProductDBParameters parameters, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            "warehouse.usp_CreateProduct",
            cancellationToken,
            new
            {
                parameters.ProductName,
                parameters.ProductDesc,
                parameters.ProductCountries
            });

        _activity.WhenCapturingSqlException(sqlEx => _logger.LogError("Failed to create product."));

        var productId = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<int>(cmd);
        });

        return new CreateProductDBResponse { ProductId = productId };
    }
}