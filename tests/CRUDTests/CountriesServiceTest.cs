using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        _countriesService = new CountriesService();
    }

    // When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public void AddCountry_Null_Exception()
    {
        //Arrange
        CountryAddRequest? countryAddRequest = null;

        Assert.Throws<ArgumentNullException>(() => _countriesService.AddCountry(countryAddRequest));
    }

    //When the CountryName is null, it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request);
        });
    }

    //When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        //Arrange
        CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
        CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
        });
    }

    //When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        //Arrange
        CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

        //Act
        CountryResponse response = _countriesService.AddCountry(request);

        //Assert
        Assert.True(response.CountryID != Guid.Empty);
    }

    [Fact]
    public void GetCountryByCountryID_NullCountryID_Null()
    {
        //Arrange
        Guid? countryID = null;

        //Act
        CountryResponse? response = _countriesService.GetCountryByCountryID(countryID);

        //Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetCountryByCountryID_ValidCountryID_Country()
    {
        //Arrange
        CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        //Act
        CountryResponse? country_response_from_get = _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

        //Assert
        Assert.Equal(country_response_from_add, country_response_from_get);
    }

    [Fact]
    //The list of countries should be empty by default (before adding any countries)
    public void GetAllCountries_EmptyList()
    {
        //Act
        List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

        //Assert
        Assert.Empty(actual_country_response_list);
    }
}