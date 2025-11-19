using AutoFixture;
using CRUDOperationSystem.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTest
{
    public class PersonsControllerTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ICountriesService> _countriesServieceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _countriesServieceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();
            _countriesService = _countriesServieceMock.Object;
            _personsService = _personsServiceMock.Object;
            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;
            _fixture = new Fixture();
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            _personsServiceMock.Setup(temp =>
            temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponses);

            _personsServiceMock.Setup(temp =>
            temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(),
            It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(personResponses);

            IActionResult result =
            await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponses);
        }
        #endregion

        #region Creat
        [Fact]
        public async Task Create_IfModelErrors_ToReturnCreateView()
        {
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countriesResponse = _fixture.Create<List<CountryResponse>>();

            _countriesServieceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countriesResponse);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");


            IActionResult result =
            await personsController.Create(personAddRequest);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(personAddRequest);
        }

        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnCreateView()
        {
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countriesResponse = _fixture.Create<List<CountryResponse>>();

            _countriesServieceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countriesResponse);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            IActionResult result =
            await personsController.Create(personAddRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }


        #endregion
    }
}
