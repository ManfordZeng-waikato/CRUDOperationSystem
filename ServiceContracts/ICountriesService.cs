using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a Country object to the list of Countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add </param>
        /// <returns>The country object after adding (including newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as list of CountryResponse</returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Returns a country based on the given countryID
        /// </summary>
        /// <param name="countryID">GUID ID</param>
        /// <returns>Matching country as CountryResponse object</returns>
        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}
