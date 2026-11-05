namespace InventoryService.Features.Countries.GetCountries;
/// <summary>
/// A representation of the available countries in the database.
/// </summary>
/// <param name="Countries">A collection of countries.</param>
public record GetCountriesDBResponse
(
    ICollection<GetCountriesDBCountriesResponse> Countries
);

/// <summary>
/// A database represenation of a country.
/// </summary>
/// <param name="CountryId">The Country's Country Id.</param>
/// <param name="CountryCode">The Country's Country Code.</param>
/// <param name="CurrencyCode">The County's Currency Code.</param>
public record GetCountriesDBCountriesResponse
(
    int CountryId,
    string CountryCode,
    string CurrencyCode
);