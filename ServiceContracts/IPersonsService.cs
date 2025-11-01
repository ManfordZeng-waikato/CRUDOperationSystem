using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        List<PersonResponse> GetAllPersons();
        PersonResponse? GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchBy">Field to search</param>
        /// <param name="searchString">Content to search</param>
        /// <returns></returns>
        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

    }
}
