using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDOperationSystem.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {

        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
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

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countryResponses =
            _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }
            );
            //new SelectListItem() { Text="USA",Value="1122233"};
            //<option value="1122233"></option>
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countryResponses =
                _countriesService.GetAllCountries();
                ViewBag.Countries = countryResponses;

                ViewBag.Errors = ModelState.Values.SelectMany(v =>
                v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            PersonResponse personResponse =
            _personsService.AddPerson(personAddRequest);

            //Made another get request to "persons/index", navigae to Index() action method
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse =
            _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countryResponses =
           _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            }
            );

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =
            _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatePerson =
                _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countryResponses =
                _countriesService.GetAllCountries();
                ViewBag.Countries = countryResponses;

                ViewBag.Errors = ModelState.Values.SelectMany(v =>
                v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public IActionResult Delete(Guid? personID)
        {
            PersonResponse? personResponse =
            _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =
            _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");
            _personsService.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index");
        }
    }
}
