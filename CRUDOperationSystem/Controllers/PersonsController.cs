using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDOperationSystem.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrderOptions = SortOrderOptions.ASC)
        {
            //Search
            ViewBag.SearchFileds = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName) ,"Person Name" },
                { nameof(PersonResponse.Email) ,"Email" },
                { nameof(PersonResponse.DateOfBirth) ,"Date of Birth" },
                { nameof(PersonResponse.Gender) ,"Gender" },
                { nameof(PersonResponse.CountryID) ,"Country ID" },
                { nameof(PersonResponse.Address) ,"Address" },
            };

            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPerson = _personsService.GetSortedPersons(persons, sortBy, sortOrderOptions);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOptions.ToString();

            return View(sortedPerson);
        }

        [Route("persons/create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countryResponses =
            _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses;
            return View();
        }
    }
}
