using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsGetterService
    {
        
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy">Field to search</param>
        /// <param name="searchString">Content to search</param>
        /// <returns></returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        Task<MemoryStream> GetPersonsCSV();

        Task<MemoryStream> GetPersonsExcel();
    }
}
