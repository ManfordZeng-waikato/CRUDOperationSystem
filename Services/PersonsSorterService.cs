using Microsoft.Extensions.Logging;
using RepositoryContract;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonsSorterService : IPersonsSorterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsSorterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string sortBy, SortOrderOptions sortOrder)
        {

            if (sortBy == null)
                return allpersons;

            return await Task.Run(() =>
             {
                 List<PersonResponse> SortedPersons = (sortBy, sortOrder) switch
                 {
                     (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.PersonName,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.PersonName,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Email,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                      allpersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                     (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                     (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                      allpersons.OrderBy(temp => temp.Age).ToList(),

                     (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Age).ToList(),

                     (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Gender,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Gender,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Country,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Country,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Address,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Address,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                     (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                     _ => allpersons
                 };

                 return SortedPersons;
             });
        }


    }
}
