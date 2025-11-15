using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock =
            new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(null);

            _personService = new PersonsService(null);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            PersonAddRequest? personAddRequest = null;
            Func<Task> action = async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Func<Task> action = async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "example@gg.com")
                .Create();
            PersonResponse person_response_from_add =
              await _personService.AddPerson(personAddRequest);

            List<PersonResponse> persons_list =
           await _personService.GetAllPersons();

            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

            //Assert.Contains(person_response_from_add, persons_list);
            persons_list.Should().Contain(person_response_from_add);
        }

        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            Guid personID = Guid.Empty;

            PersonResponse? person_response_from_get =
           await _personService.GetPersonByPersonID(personID);

            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            CountryAddRequest countryAddRequest =
                _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sample@gg.com")
                .Create();
            PersonResponse personResponse =
           await _personService.AddPerson(personAddRequest);

            PersonResponse? personResponseFromGet =
           await _personService.GetPersonByPersonID(personResponse.PersonID);

            //Assert.Equal(personResponse, personResponseFromGet);
            personResponseFromGet.Should().Be(personResponse);
        }

        #endregion

        #region GetAllPersons
        //Should return an empty list by default 
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            List<PersonResponse> personResponsesDefault =
               await _personService.GetAllPersons();

            //Assert.Empty(personResponsesDefault);
            personResponsesDefault.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest countryCodeAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryCodeAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "222@gg.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "333@gg.com")
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>();
            foreach (PersonAddRequest person_request in personAddRequests)
            {
                PersonResponse person_reponse =
              await _personService.AddPerson(person_request);
                personResponsesFromAdd.Add(person_reponse);
            }

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

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
        public async Task GetFilteredPersons_EmptySearchText()
        {
            CountryAddRequest countryCodeAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryCodeAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "222@gg.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "333@gg.com")
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>();
            foreach (PersonAddRequest person_request in personAddRequests)
            {
                PersonResponse person_reponse =
               await _personService.AddPerson(person_request);
                personResponsesFromAdd.Add(person_reponse);
            }

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

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

            persons_list_from_search.Should().BeEquivalentTo(personResponsesFromAdd);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest countryCodeAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryCodeAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.PersonName, "wenji")
                .With(p => p.CountryID, countryResponse2.CountryID)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.PersonName, "Manford")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.PersonName, "Manford2")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>();
            foreach (PersonAddRequest person_request in personAddRequests)
            {
                PersonResponse person_reponse =
               await _personService.AddPerson(person_request);
                personResponsesFromAdd.Add(person_reponse);
            }

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

            List<PersonResponse> persons_list_from_search =
           await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            //print personResponsesFromGet
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine((person_response_from_get).ToString());
            }

            /* foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
             {
                 if (person_response_from_add.PersonName != null)
                 {
                     if (person_response_from_add.PersonName.Contains("ma",
                                        StringComparison.OrdinalIgnoreCase))
                     {
                         Assert.Contains(person_response_from_add, persons_list_from_search);
                     }
                 }
             }*/

            persons_list_from_search.Should().OnlyContain(temp =>
            temp.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region GetSortedPersons
        //When we sort based on PersonName in DESC, it should return persons list in descending on person name
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest countryCodeAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryCodeAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "222@gg.com")
                .With(p => p.PersonName, "wenji")
                .With(p => p.CountryID, countryResponse2.CountryID)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "333@gg.com")
                .With(p => p.PersonName, "Manford")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "444@gg.com")
                .With(p => p.PersonName, "Manford2")
                .With(p => p.CountryID, countryResponse1.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>();
            foreach (PersonAddRequest person_request in personAddRequests)
            {
                PersonResponse person_reponse =
               await _personService.AddPerson(person_request);
                personResponsesFromAdd.Add(person_reponse);
            }
            List<PersonResponse> allpersons = await _personService.GetAllPersons();

            //print personResponsesFromAdd
            _testOutputHelper.WriteLine("Expected:");
            personResponsesFromAdd = personResponsesFromAdd.OrderByDescending(temp =>
           temp.PersonName).ToList();
            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                _testOutputHelper.WriteLine((person_response_from_add).ToString());
            }

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
        public async Task UpdatePerson_NullPerson()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = async () =>
                {
                    await _personService.UpdatePerson(personUpdateRequest);
                };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
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
        public async Task UpdatePerson_NullPersonName()
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

