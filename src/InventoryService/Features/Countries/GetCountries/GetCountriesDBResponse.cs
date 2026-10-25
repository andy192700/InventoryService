namespace InventoryService.Features.Countries.GetCountries;

public class GetCountriesDBResponse
{
    public ICollection<GetCountriesDBCountriesResponse> Countries { get; set; } = [];
}

public class GetCountriesDBCountriesResponse
{
    public int CountryId { get; set; }

    public required string CountryCode { get; set; }

    public required string CurrencyCode { get; set; }
}