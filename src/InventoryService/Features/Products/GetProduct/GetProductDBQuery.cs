using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Products.GetProduct;

public interface IGetProductDBQuery
{
    Task<GetProductDBResponse> GetProductAsync(
        GetProductDBParameters dBParameters, 
        CancellationToken cancellationToken);
}

public class GetProductDBQuery : IGetProductDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetProductDBQuery(ISQLConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
    }

    public async Task<GetProductDBResponse> GetProductAsync(
        GetProductDBParameters dBParameters,
        CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetProduct",
            param: new
            {
                dBParameters.ProductId
            },
            commandType: CommandType.StoredProcedure);

        var product = await result.ReadFirstOrDefaultAsync<GetProductDBProductResponse>();

        var productCountries = (await result.ReadAsync<GetProductDbProductCountriesResponse>()).ToList();

        return new GetProductDBResponse
        {
            Product = product,
            ProductCountries = productCountries
        };
    }
}
