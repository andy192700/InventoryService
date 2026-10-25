namespace InventoryService.Features.Countries.GetCountries;

public class GetCountriesResponse
{
    public int CountryId { get; set; }

    public required string CountryCode { get; set; }

    public required string CurrencyCode { get; set; }
}
