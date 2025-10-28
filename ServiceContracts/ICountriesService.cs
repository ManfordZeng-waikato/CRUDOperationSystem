﻿using ServiceContracts.DTO;

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
        /// <returns>The country objec after adding (including newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);
    }
}
