using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy">Field to search</param>
        /// <param name="searchString">Content to search</param>
        /// <returns></returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// /
        /// </summary>
        /// <param name="allpersons">List of persons to be sorted </param>
        /// <param name="sortBy">Name of the property(key)</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string sortBy,
             SortOrderOptions sortOrder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, including person ID</param>
        /// <returns></returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>true:the deletion is successful, otherwise unsuccessful</returns>
        Task<bool> DeletePerson(Guid? personID);

        Task<MemoryStream> GetPersonsCSV();
    }
}
