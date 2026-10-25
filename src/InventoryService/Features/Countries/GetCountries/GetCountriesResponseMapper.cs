namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// A contract for mapping a countries database response to a collection of <see cref="GetCountriesResponse"/>. 
/// </summary>
public interface IGetCountriesResponseMapper
{
    /// <summary>
    /// Map the countries database response to a collection of countries responses.
    /// </summary>
    /// <param name="dbResponse">The database response.</param>
    /// <returns>The collection of countries.</returns>
    ICollection<GetCountriesResponse> Map(GetCountriesDBResponse dbResponse);
}

/// <summary>
/// Concrete implementation for a <see cref="IGetCountriesResponseMapper"/>.
/// </summary>
public class GetCountriesResponseMapper : IGetCountriesResponseMapper
{
    /// <inheritdoc/>
    public ICollection<GetCountriesResponse> Map(GetCountriesDBResponse dbResponse)
    {
        var countries = new List<GetCountriesResponse>();

        foreach (var country in dbResponse.Countries)
        {
            countries.Add(new GetCountriesResponse
            {
                CountryId = country.CountryId,
                CountryCode = country.CountryCode,
                CurrencyCode = country.CurrencyCode,
            });
        }

        return countries;
    }
}
