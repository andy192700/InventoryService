using Dapper;
using InventoryService.Infrastructure;
using System.Data;

namespace InventoryService.Features.Countries.GetCountries;

public interface IGetCountriesDBQuery
{
    Task<GetCountriesDBResponse> GetCountriesAsync(CancellationToken cancellationToken);
}

public class GetCountriesDBQuery : IGetCountriesDBQuery
{
    private ISQLConnectionProvider _connectionProvider;

    public GetCountriesDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<GetCountriesDBResponse> GetCountriesAsync(CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "dbo.usp_GetCountries",
            commandType: CommandType.StoredProcedure);

        var countries = (await result.ReadAsync<GetCountriesDBCountriesResponse>()).ToList();

        return new GetCountriesDBResponse
        {
            Countries = countries
        };
    }
}
