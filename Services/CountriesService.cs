using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>() {
                new Country() { CountryID = Guid.Parse("EF815347-9CDF-4439-A252-5BE0CF7EC6EC"), CountryName = "USA" },

                new Country() { CountryID = Guid.Parse("27D3D72A-1246-4356-9112-5F7ED93CC982"), CountryName = "CHINA" },

                new Country() { CountryID = Guid.Parse("C7D71F7E-5B1A-4B76-AB4A-A087FCDEE4E9"), CountryName = "NZ" },

                new Country() { CountryID = Guid.Parse("E41F64D8-67EB-4A46-839E-2ABC21CBA364"), CountryName = "AUS" },

                new Country() { CountryID = Guid.Parse("C4FFB0D1-19FE-46F0-B0DD-BF16BBD8958D"), CountryName = "JAPAN" },

                new Country() { CountryID = Guid.Parse("EB4DE433-A846-433D-B6A2-0104F519146D"), CountryName = "KOREA" },
                });
            }
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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

            if (_countries.Where(country =>
            country.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }
            //add the country to the existing list of countries
            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            _countries.Add(country);
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country =>
            country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? country_response_from_list =
                 _countries.FirstOrDefault(temp =>
                 temp.CountryID == countryID);
            if (country_response_from_list == null)
            {
                return null;
            }
            return country_response_from_list.ToCountryResponse();
        }
    }
}
