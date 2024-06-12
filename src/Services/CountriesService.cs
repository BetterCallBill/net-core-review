﻿using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;

    //constructor
    public CountriesService(bool initialize = true)
    {
        _countries = new List<Country>();
        if (initialize)
        {
            _countries.AddRange(new List<Country>() {
                new() {  CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA" },

                new() { CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

                new() { CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

                new() { CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "India" },

                new() { CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
            });
        }
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
