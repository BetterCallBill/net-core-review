using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService()
    {
        _countries = new List<Country>();
    }

    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest is null)
            throw new ArgumentNullException(nameof(countryAddRequest));
            
        if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            throw new ArgumentException("CountryName cannot be null or empty", nameof(countryAddRequest));
            
        if (_countries.Any(c => c.CountryName == countryAddRequest.CountryName))
            throw new ArgumentException("CountryName already exists", nameof(countryAddRequest));
            
        Country country = countryAddRequest.ToCountry();
        country.CountryID = Guid.NewGuid();
        _countries.Add(country);
        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        throw new NotImplementedException();
    }

    public CountryResponse? GetCountryByCountryID(Guid? countryID)
    {
        throw new NotImplementedException();
    }
}
