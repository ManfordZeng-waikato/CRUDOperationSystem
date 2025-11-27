using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContract;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesUploaderService _countriesUploaderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAdderService _countriesAdderService;

        private readonly IFixture _fixture;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesUploaderService = new CountriesUploaderService(_countriesRepository);
            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesGetterService = new CountriesGetterService(_countriesRepository);

            _fixture = new Fixture();
        }

        #region AddCountry
        //when CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            Country country = _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>).Create();
            _countriesRepositoryMock
           .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
           .ReturnsAsync(country);

            //Act
            var action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }



        //when ContryName is null,it should throw ArgumentNullException

        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();
            Country country = _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>).Create();

            _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);
            //Assert
            var action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //when ContryName is duplicate,it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {

            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "USA")
                .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "USA")
                .Create();
            Country country1 = request1.ToCountry();
            Country country2 = request2.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);
            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country =
            await _countriesAdderService.AddCountry(request1);
            //Assert
            //Act
            var action = async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country1);

                await _countriesAdderService.AddCountry(request2);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //when you supply propper  countryName,it should add the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .Create();
            Country country = request.ToCountry();
            CountryResponse countryResponse = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);

            //Act
            CountryResponse countryResponseFromAdd = await _countriesAdderService.AddCountry(request);
            country.CountryID = countryResponseFromAdd.CountryID;
            countryResponse.CountryID = countryResponseFromAdd.CountryID;
            //Assert
            countryResponseFromAdd.CountryID.Should().NotBe(Guid.Empty);
            countryResponseFromAdd.Should().BeEquivalentTo(countryResponse);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        //The List of country should be empty by default
        public async Task GetAllcountries_ToBeEmptyList()
        {
            List<Country> country_empty_list = new List<Country>();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            //Assert
            actual_country_response_list.Should().BeEmpty();

        }

        [Fact]
        public async Task GetAllcountries_ShouldHaveFewCountries()
        {
            //Arrange
            List<Country> country_list = new List<Country>() {
            _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>).Create(),
            _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>).Create()
            };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

            //Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? countryResponse =
           await _countriesGetterService.GetCountryByCountryID(countryID);

            //Assert
            countryResponse.Should().BeNull();

            _countriesRepositoryMock.Verify(
            x => x.GetCountryByCountryID(It.IsAny<Guid>()),
            Times.Never);
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
            .With(temp => temp.Person, null as List<Person>)
            .Create();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryID(country.CountryID))
            .ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get = await _countriesGetterService.GetCountryByCountryID(country.CountryID);

            //Assert
            country_response_from_get.Should().BeEquivalentTo(country_response);
        }
        #endregion

        #region
        #endregion
    }
}
