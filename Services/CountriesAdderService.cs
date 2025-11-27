using Entities;
using RepositoryContract;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
            }

            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }
            //add the country to the existing list of countries
            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }
    }
}
