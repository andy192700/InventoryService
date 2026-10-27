using Dapper;
using System.Data;

namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// Contract for retrieving the available countries for the database.
/// </summary>
public interface IGetCountriesDBQuery
{
    /// <summary>
    /// Retrieves the available countries from the database..
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A Task of type <see cref="GetCountriesDBResponse"/>, containing the database response.</returns>
    Task<GetCountriesDBResponse> GetCountriesAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Concrete implementation of a <see cref="IGetCountriesDBQuery"/>, which uses a <see cref="ISQLConnectionProvider"/>
/// to obtain a SQL connection.
/// </summary>
public class GetCountriesDBQuery : IGetCountriesDBQuery
{
    private ISQLConnectionProvider _connectionProvider;

    public GetCountriesDBQuery(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    /// <inheritdoc/>
    public async Task<GetCountriesDBResponse> GetCountriesAsync(CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        using var result = await conn.QueryMultipleAsync(
            "warehouse.usp_GetCountries",
            commandType: CommandType.StoredProcedure);

        var countries = (await result.ReadAsync<GetCountriesDBCountriesResponse>()).ToList();

        return new GetCountriesDBResponse
        {
            Countries = countries
        };
    }
}
