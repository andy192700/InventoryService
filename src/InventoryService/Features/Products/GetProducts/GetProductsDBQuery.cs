using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Products.GetProducts;

public interface IGetProductsDBQuery
{
    Task<GetProductsDBResponse> GetProductsAsync(GetProductsDBParameters dbParameters, CancellationToken cancellationToken);
}

public class GetProductsDBQuery : IGetProductsDBQuery
{
    private readonly ISQLConnectionProvider _sqlConnectionProvider;

    public GetProductsDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _sqlConnectionProvider = connectionProvider;
    }

    public async Task<GetProductsDBResponse> GetProductsAsync(GetProductsDBParameters dbParameters, CancellationToken cancellationToken)
    {
        using var conn = await _sqlConnectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetProducts",
            param: new
            {
                dbParameters.Filter
            },
            commandType: CommandType.StoredProcedure);

        var products = (await result.ReadAsync<GetProductsDBProductsResponse>()).ToList(); 

        var productCountries = (await result.ReadAsync<GetProductsDbProductCountriesResponse>()).ToList();

        return new GetProductsDBResponse
        {
            Products = products,
            ProductCountries = productCountries
        };
    }
}
