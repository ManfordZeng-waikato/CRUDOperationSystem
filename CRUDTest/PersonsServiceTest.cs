using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContract;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        private readonly IPersonsRepository _personRepository;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personRepository = _personRepositoryMock.Object;

            _fixture = new Fixture();


            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock =
            new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(null);

            _personService = new PersonsService(_personRepository);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;
            Func<Task> action = async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();
            Person person = personAddRequest.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            Func<Task> action = async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails_ToBeSuccessful()
        {
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "example@gg.com")
                .Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse person_response_from_add =
              await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;


            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);
        }

        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            Guid personID = Guid.Empty;

            PersonResponse? person_response_from_get =
           await _personService.GetPersonByPersonID(personID);

            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {


            Person person_request = _fixture.Build<Person>()
                .With(p => p.Email, "sample@gg.com")
                .With(p => p.Country, null as Country)
                .Create();
            PersonResponse person_response_expected = person_request.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(person_request.PersonID))
                .ReturnsAsync(person_request);

            PersonResponse? personResponseFromGet =
           await _personService.GetPersonByPersonID(person_request.PersonID);

            //Assert.Equal(personResponse, personResponseFromGet);
            personResponseFromGet.Should().Be(person_response_expected);
        }

        #endregion

        #region GetAllPersons
        //Should return an empty list by default 
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(new List<Person>());

            List<PersonResponse> personResponsesDefault =
               await _personService.GetAllPersons();

            //Assert.Empty(personResponsesDefault);
            personResponsesDefault.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>(){
                _fixture.Build<Person>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
             _fixture.Build<Person>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.Country, null as Country)
                .Create() };


            List<PersonResponse> personResponsesFromAdd = persons.Select(p =>
            p.ToPersonResponse()).ToList();

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> persons_list_from_get =
          await _personService.GetAllPersons();

            //print personResponsesFromGet
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine((person_response_from_get).ToString());
            }

            /*foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                //Assert.Contains(person_response_from_add, persons_list_from_get);
            }*/
            persons_list_from_get.Should().BeEquivalentTo(personResponsesFromAdd);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>(){
                _fixture.Build<Person>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
             _fixture.Build<Person>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.Country, null as Country)
                .Create() };

            List<PersonResponse> personResponsesExpected = persons.Select(p =>
            p.ToPersonResponse()).ToList();

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesExpected)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

            _personRepositoryMock
           .Setup(r => r.GetAllPersons())
           .ReturnsAsync(persons);

            _personRepositoryMock.Setup(temp =>
            temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            List<PersonResponse> persons_list_from_search =
           await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            //print personResponsesFromGet
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine((person_response_from_get).ToString());
            }

            /*    foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }*/

            persons_list_from_search.Should().BeEquivalentTo(personResponsesExpected);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>(){
                _fixture.Build<Person>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
             _fixture.Build<Person>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.Country, null as Country)
                .Create() };

            List<PersonResponse> personResponsesExpected = persons.Select(p =>
            p.ToPersonResponse()).ToList();

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesExpected)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

            _personRepositoryMock
           .Setup(r => r.GetAllPersons())
           .ReturnsAsync(persons);

            _personRepositoryMock.Setup(temp =>
            temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            List<PersonResponse> persons_list_from_search =
           await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print personResponsesFromGet
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine((person_response_from_get).ToString());
            }

            /*    foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }*/

            persons_list_from_search.Should().BeEquivalentTo(personResponsesExpected);
        }
        #endregion

        #region GetSortedPersons
        //When we sort based on PersonName in DESC, it should return persons list in descending on person name
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>(){
                _fixture.Build<Person>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
             _fixture.Build<Person>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.Country, null as Country)
                .Create() };

            List<PersonResponse> personResponsesExpected = persons.Select(p =>
            p.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");

            foreach (PersonResponse person_response_from_add in personResponsesExpected)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

            List<PersonResponse> allpersons = await _personService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort =
           await _personService.GetSortedPersons(allpersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print personResponsesFromGet
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_sort in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine((person_response_from_sort).ToString());
            }


            /*for (int i = 0; i < personResponsesFromAdd.Count; i++)
            {
                Assert.Equal(personResponsesFromAdd[i], persons_list_from_sort[i]);
            }
*/
            persons_list_from_sort.Should().BeInDescendingOrder(p => p.PersonName);
        }
        #endregion

        #region UpdatePersons
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = async () =>
                {
                    await _personService.UpdatePerson(personUpdateRequest);
                };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            PersonUpdateRequest personUpdateRequest =
                _fixture.Build<PersonUpdateRequest>()
                .Create();
            Func<Task> action = async () =>
              {
                  await _personService.UpdatePerson(personUpdateRequest);
              };
            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {


            CountryAddRequest countryCodeAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse =
           await _countriesService.AddCountry(countryCodeAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.PersonName, "Manford2")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse person_response_from_add =
           await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest =
                person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            Func<Task> action = async () =>
              {
                  await _personService.UpdatePerson(personUpdateRequest);
              };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetails()
        {
            CountryAddRequest countryCodeAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse =
           await _countriesService.AddCountry(countryCodeAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.PersonName, "Manford2")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse person_response_from_add =
           await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest =
                person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Will";
            personUpdateRequest.Email = "hhh@gmail.com";

            PersonResponse person_response_from_update =
               await _personService.UpdatePerson(personUpdateRequest);

            PersonResponse? person_response_from_get =
               await _personService.GetPersonByPersonID(person_response_from_update.PersonID);
            //Assert.Equal(person_response_from_get, person_response_from_update);
            person_response_from_update.Should()
                .BeEquivalentTo(person_response_from_get);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            CountryAddRequest countryCodeAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse =
           await _countriesService.AddCountry(countryCodeAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.PersonName, "Manford2")
                .With(p => p.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse person_response_from_add =
           await _personService.AddPerson(personAddRequest);

            bool isDelete =
           await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert.True(isDelete);
            isDelete.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            bool isDelete = await _personService.DeletePerson(Guid.NewGuid());

            //Assert.False(isDelete);
            isDelete.Should().BeFalse();
        }
        #endregion
    }
}

