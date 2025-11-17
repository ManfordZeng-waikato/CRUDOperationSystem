using AutoFixture;
using CRUDOperationSystem.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        private readonly Mock<ICountriesService> _countriesServieceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _countriesServieceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();
            _countriesService = _countriesServieceMock.Object;
            _personsService = _personsServiceMock.Object;
            _fixture = new Fixture();
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService);

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
    }
}
