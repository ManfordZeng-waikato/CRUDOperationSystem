using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsSorterService
    {
        /// <summary>
        /// /
        /// </summary>
        /// <param name="allpersons">List of persons to be sorted </param>
        /// <param name="sortBy">Name of the property(key)</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string sortBy,
             SortOrderOptions sortOrder);
    }
}
