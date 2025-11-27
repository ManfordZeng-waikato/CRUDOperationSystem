using Entities;
using RepositoryContract;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries())
                .OrderBy(c => c.CountryName)
                .Select(country =>
            country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? country_response_from_list =
                await _countriesRepository.GetCountryByCountryID(countryID.Value);
            if (country_response_from_list == null)
            {
                return null;
            }
            return country_response_from_list.ToCountryResponse();
        }
    }
}
