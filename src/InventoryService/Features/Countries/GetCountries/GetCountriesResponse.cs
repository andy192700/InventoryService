namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// An individual Country response, returned by the <see cref="GetCountriesHandler"/>
/// </summary>
/// <param name="CountryId">The Country's Country Id.</param>
/// <param name="CountryCode">The Country's Country Code.</param>
/// <param name="CurrencyCode">The Country's Currency Code.</param>
public record GetCountriesResponse
(
    int CountryId,
    string CountryCode,
    string CurrencyCode
);
