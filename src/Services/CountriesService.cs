using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PersonsDbContext _dbContext;


    //constructor
    public CountriesService(PersonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        List<CountryResponse> countries = _dbContext.Countries.ToList().Select(country => country.ToCountryResponse()).ToList();

        if (countryAddRequest is null)
            throw new ArgumentNullException(nameof(countryAddRequest));

        if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            throw new ArgumentException("CountryName cannot be null or empty", nameof(countryAddRequest));

        if (countries.Any(c => c.CountryName == countryAddRequest.CountryName))
            throw new ArgumentException("CountryName already exists", nameof(countryAddRequest));

        Country country = countryAddRequest.ToCountry();
        country.CountryID = Guid.NewGuid();

        _dbContext.Countries.Add(country);
        _dbContext.SaveChanges();

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _dbContext.Countries.ToList().Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
            return null;

        Country? country_response_from_list = _dbContext.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

        if (country_response_from_list == null)
            return null;

        return country_response_from_list.ToCountryResponse();
    }
}
