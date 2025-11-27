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
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;

        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ICountriesService> _countriesServieceMock;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _countriesServieceMock = new Mock<ICountriesService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();

            _countriesService = _countriesServieceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;
            _fixture = new Fixture();
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsSorterService, _personsUpdaterService, _countriesService, _logger);

            _personsGetterServiceMock.Setup(temp =>
            temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponses);

            _personsSorterServiceMock.Setup(temp =>
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
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndexView()
        {
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countriesResponse = _fixture.Create<List<CountryResponse>>();

            _countriesServieceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countriesResponse);
            _personsAdderServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsGetterService, _personsAdderService, _personsDeleterService, _personsSorterService, _personsUpdaterService, _countriesService, _logger);

            IActionResult result =
            await personsController.Create(personAddRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
