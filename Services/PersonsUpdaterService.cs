using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContract;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchimgPerson =
            await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (matchimgPerson == null)
                throw new InvalidPersonIDException("Given person ID doesn't exist");

            matchimgPerson.PersonName = personUpdateRequest.PersonName;
            matchimgPerson.Gender = personUpdateRequest.Gender.ToString();
            matchimgPerson.Email = personUpdateRequest.Email;
            matchimgPerson.Address = personUpdateRequest.Address;
            matchimgPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchimgPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            matchimgPerson.CountryID = personUpdateRequest.CountryID;
            await _personsRepository.UpdatePerson(matchimgPerson);

            return matchimgPerson.ToPersonResponse();
        }

    }
}
