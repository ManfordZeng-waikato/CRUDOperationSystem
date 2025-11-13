using Entities;
using EntityFrameworkCoreMock;
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
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock =
            new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(dbContext);

            _personService = new PersonsService(dbContext, _countriesService);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            PersonAddRequest? personAddRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
              {
                  await _personService.AddPerson(personAddRequest);
              });
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 await _personService.AddPerson(personAddRequest);
             });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
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
              await _personService.AddPerson(personAddRequest);

            List<PersonResponse> persons_list =
           await _personService.GetAllPersons();

            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, persons_list);

        }

        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            Guid personID = Guid.Empty;

            PersonResponse? person_response_from_get =
           await _personService.GetPersonByPersonID(personID);

            Assert.Null(person_response_from_get);
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Canada"
            };
            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);

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
           await _personService.AddPerson(personAddRequest);

            PersonResponse? personResponseFromGet =
           await _personService.GetPersonByPersonID(personResponse.PersonID);

            Assert.Equal(personResponse, personResponseFromGet);
        }

        #endregion

        #region GetAllPersons
        //Should return an empty list by default 
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            List<PersonResponse> personResponsesDefault =
               await _personService.GetAllPersons();

            Assert.Empty(personResponsesDefault);
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "jjj@gg.com",
                Address = "address",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Manford1",
                Email = "jjj@gg.com",
                Address = "address2",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Manford3",
                Email = "jjj@gg.com",
                Address = "address3",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

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

            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponse1 =
          await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "jjj@gg.com",
                Address = "address",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Manford1",
                Email = "jjj@gg.com",
                Address = "address2",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Manford3",
                Email = "jjj@gg.com",
                Address = "address3",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

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

            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "jjj@gg.com",
                Address = "address",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Manford1",
                Email = "jjj@gg.com",
                Address = "address2",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Manford3",
                Email = "jjj@gg.com",
                Address = "address3",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

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

            foreach (PersonResponse person_response_from_add in personResponsesFromAdd)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma",
                                       StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }
            }
        }
        #endregion

        #region GetSortedPersons
        //When we sort based on PersonName in DESC, it should return persons list in descending on person name
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest countryCodeAddRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryCodeAddRequest2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponse1 =
           await _countriesService.AddCountry(countryCodeAddRequest1);
            CountryResponse countryResponse2 =
           await _countriesService.AddCountry(countryCodeAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Manford",
                Email = "jjj@gg.com",
                Address = "address",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Manford1",
                Email = "jjj@gg.com",
                Address = "address2",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Manford3",
                Email = "jjj@gg.com",
                Address = "address3",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("2001-09-22"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

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


            for (int i = 0; i < personResponsesFromAdd.Count; i++)
            {
                Assert.Equal(personResponsesFromAdd[i], persons_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePersons
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
              {
                  await _personService.UpdatePerson(personUpdateRequest);
              });
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
              {
                  await _personService.UpdatePerson(personUpdateRequest);
              });

        }

        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add =
            await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email = "mmm@kk.com",
                Gender = GenderOptions.Male,
            };
            PersonResponse person_response_from_add =
           await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest =
                person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            await Assert.ThrowsAsync<ArgumentException>(async () =>
              {
                  await _personService.UpdatePerson(personUpdateRequest);
              });
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetails()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add =
            await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Address = "here",
                Email = "aaa@gg.com",
                DateOfBirth = DateTime.Now,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
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
            Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country_response_from_add =
           await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email = "mmm@kk.com",
                Gender = GenderOptions.Male,
                Address = "sss",
                ReceiveNewsLetters = true,
            };
            PersonResponse person_response_from_add =
           await _personService.AddPerson(personAddRequest);

            bool isDelete =
           await _personService.DeletePerson(person_response_from_add.PersonID);

            Assert.True(isDelete);
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            bool isDelete = await _personService.DeletePerson(Guid.NewGuid());

            Assert.False(isDelete);
        }
        #endregion
    }
}

