using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock =
            new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountry
        //when CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;


            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
              {
                  //Act
                  await _countriesService.AddCountry(request);

              });

        }

        //when ContryName is null,it should throw ArgumentNullException

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
             {
                 //Act
                 await _countriesService.AddCountry(request);

             });

        }

        //when ContryName is duplicate,it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "USA")
                .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "USA")
                .Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 //Act
                 await _countriesService.AddCountry(request1);
                 await _countriesService.AddCountry(request2);

             });

        }

        //when you supply propper  countryName,it should add the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .Create();

            //Act
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCoutries =
               await _countriesService.GetAllCountries();

            //Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countries_from_GetAllCoutries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        //The List of country should be empty by default
        public async Task GetAllcountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);

        }

        [Fact]
        public async Task GetAllcountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest (){ CountryName="USA"},
                new CountryAddRequest (){ CountryName="UK"},
            };
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
            foreach (CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add
               (await _countriesService.AddCountry(country_request));
            }

            //Act
            List<CountryResponse> actual_country_response_listt =
           await _countriesService.GetAllCountries();

            //read each element from countries_list_from_add_country
            foreach (CountryResponse expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actual_country_response_listt);
            }
        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? countryResponse =
           await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add =
               await _countriesService.AddCountry(request1);

            //Act
            CountryResponse? country_response_from_get =
            await _countriesService.GetCountryByCountryID
                (country_response_from_add.CountryID);

            //Assert
            Assert.Equal(country_response_from_add, country_response_from_get);
        }
        #endregion

        #region
        #endregion
    }
}
