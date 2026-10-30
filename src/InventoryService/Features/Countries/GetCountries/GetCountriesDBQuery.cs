using Dapper;
using InventoryService.Extensions;
using InventoryService.Infrastructure;

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
    private ISQLActivity _activity;

    public GetCountriesDBQuery(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <inheritdoc/>
    public async Task<GetCountriesDBResponse> GetCountriesAsync(CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync("warehouse.usp_GetCountries", cancellationToken);

        var countries = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            using var result = await connection.QueryMultipleAsync(cmd);

            return (await result.ReadAsync<GetCountriesDBCountriesResponse>()).ToList();
        });

        return new GetCountriesDBResponse
        {
            Countries = countries
        };
    }
}
