namespace InventoryService.Features.Countries.GetCountries;

public interface IGetCountriesResponseMapper
{
    ICollection<GetCountriesResponse> Map(GetCountriesDBResponse dbResponse);
}

public class GetCountriesResponseMapper : IGetCountriesResponseMapper
{
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
