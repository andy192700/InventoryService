namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// An individual Country response, returned by the <see cref="GetCountriesHandler"/>
/// </summary>
public class GetCountriesResponse
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
    /// The Country's Currency Code.
    /// </summary>
    public required string CurrencyCode { get; set; }
}
