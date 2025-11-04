using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.Add(new Person() { PersonID = Guid.Parse("4CC2C0AE-328E-4AA8-82CC-9818B9DDA452"), PersonName = "Pat", Email = "pshepeard0@ibm.com", DateOfBirth = DateTime.Parse("1989-03-25"), Address = "872 Carberry Junction", Gender = "Male", ReceiveNewsLetters = true, CountryID = Guid.Parse("EF815347-9CDF-4439-A252-5BE0CF7EC6EC") });

                _persons.Add(new Person() { PersonID = Guid.Parse("951C5756-445C-48C3-990C-20C7E683C55E"), PersonName = "Gaylene", Email = "bburgis2@reverbnation.com", DateOfBirth = DateTime.Parse("1988-10-28"), Address = "3443 Northview Park", Gender = "Female", ReceiveNewsLetters = false, CountryID = Guid.Parse("27D3D72A-1246-4356-9112-5F7ED93CC982") });


                _persons.Add(new Person() { PersonID = Guid.Parse("31BF5DD8-C7B9-407E-B8BE-979327280A29"), PersonName = "Brianna", Email = "gfolan1@umich.edu", DateOfBirth = DateTime.Parse("1969-12-19"), Address = "423 Killdeer Court", Gender = "Female", ReceiveNewsLetters = true, CountryID = Guid.Parse("C7D71F7E-5B1A-4B76-AB4A-A087FCDEE4E9") });


                _persons.Add(new Person() { PersonID = Guid.Parse("E5EE91A6-93F2-4BF1-A5F9-A45539FCC094"), PersonName = "Jami", Email = "jbinford3@miibeian.gov.cn", DateOfBirth = DateTime.Parse("1961-06-05"), Address = "7214 La Follette Street", Gender = "Female", ReceiveNewsLetters = false, CountryID = Guid.Parse("E41F64D8-67EB-4A46-839E-2ABC21CBA364") });


                _persons.Add(new Person() { PersonID = Guid.Parse("1F55DEE2-A7C1-4C7D-BCF9-8DFA27CA7F22"), PersonName = "Gradey", Email = "grastrick4@independent.co.uk", DateOfBirth = DateTime.Parse("1998-08-04"), Address = "8044 Blackbird Crossing", Gender = "Female", ReceiveNewsLetters = false, CountryID = Guid.Parse("C4FFB0D1-19FE-46F0-B0DD-BF16BBD8958D") });


                _persons.Add(new Person() { PersonID = Guid.Parse("7211DDF3-426F-4006-BAD1-8EE7541610CD"), PersonName = "Orville", Email = "oquayle5@boston.com", DateOfBirth = DateTime.Parse("1972-08-05"), Address = "414 Main Parkway", Gender = "Female", ReceiveNewsLetters = false, CountryID = Guid.Parse("EB4DE433-A846-433D-B6A2-0104F519146D") });
            }
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse =
           person.ToPersonResponse();
            personResponse.Country = _countriesService.
                GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            _persons.Add(person);

            return ConvertPersonToPersonResponse(person);

        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp =>
            temp.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;
            Person? person =
            _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
                return null;
            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> filteredPersons = allPersons;
            if (searchBy == null || searchString == null)
            {
                return allPersons;
            }
            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    filteredPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.PersonName) ?
                    temp.PersonName.Contains(searchString,
                    StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Email):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Email) ?
                   temp.Email.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    filteredPersons = allPersons.Where(temp =>
                   (temp.DateOfBirth != null) ?
                   temp.DateOfBirth.Value.ToString("dd MMMM, yyyy").Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.Gender):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Gender) ?
                   temp.Gender.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Country):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Country) ?
                   temp.Country.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Address):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Address) ?
                   temp.Address.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default:
                    filteredPersons = allPersons;
                    break;
            }
            return filteredPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allpersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (sortBy == null)
                return allpersons;
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
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchimgPerson =
            _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
            if (matchimgPerson == null)
                throw new ArgumentException("Given person ID doesn't exist");

            matchimgPerson.PersonName = personUpdateRequest.PersonName;
            matchimgPerson.Gender = personUpdateRequest.Gender.ToString();
            matchimgPerson.Email = personUpdateRequest.Email;
            matchimgPerson.Address = personUpdateRequest.Address;
            matchimgPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchimgPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            matchimgPerson.CountryID = personUpdateRequest.CountryID;

            return matchimgPerson.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person =
            _persons.FirstOrDefault(temp => temp.PersonID == (personID));
            if (person == null)
                return false;

            _persons.Remove(person);
            return true;
        }
    }
}
