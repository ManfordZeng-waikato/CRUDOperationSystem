using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as list of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a country based on the given countryID
        /// </summary>
        /// <param name="countryID">GUID ID</param>
        /// <returns>Matching country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
