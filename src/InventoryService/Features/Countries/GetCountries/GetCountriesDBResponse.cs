namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// A representation of the available countries in the database.
/// </summary>
public class GetCountriesDBResponse
{
    /// <summary>
    /// A collection of countries.
    /// </summary>
    public ICollection<GetCountriesDBCountriesResponse> Countries { get; set; } = [];
}

/// <summary>
/// A database represenation of a country.
/// </summary>
public class GetCountriesDBCountriesResponse
{
    /// <summary>
    /// The Country's Country Id.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// The Country's Country Code.
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    /// The County's Currency Code.
    /// </summary>
    public required string CurrencyCode { get; set; }
}