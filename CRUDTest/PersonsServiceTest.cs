using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;

        public PersonsServiceTest()
        {
            _personService = new PersonsService();
            _countriesService = new CountriesService();
        }

        #region AddPerson
        [Fact]
        public void AddPerson_NullPerson()
        {
            PersonAddRequest? personAddRequest = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            Assert.Throws<ArgumentException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "123@gg.com",
                Address = "22 Maanihi Road",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-30"),
                ReceiveNewsLetters = true
            };

            PersonResponse person_response_from_add =
                _personService.AddPerson(personAddRequest);

            List<PersonResponse> persons_list =
            _personService.GetAllPersons();

            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, persons_list);

        }

        #endregion

        #region GetPersonByPersonID
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            Guid personID = Guid.Empty;

            PersonResponse? person_response_from_get =
            _personService.GetPersonByPersonID(personID);

            Assert.Null(person_response_from_get);
        }

        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Canada"
            };
            CountryResponse countryResponse =
                _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "jjj@gg.com",
                Address = "address",
                CountryID = countryResponse.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonResponse personResponse =
            _personService.AddPerson(personAddRequest);

            PersonResponse? personResponseFromGet =
            _personService.GetPersonByPersonID(personResponse.PersonID);

            Assert.Equal(personResponse, personResponseFromGet);

        }

        #endregion
    }
}
