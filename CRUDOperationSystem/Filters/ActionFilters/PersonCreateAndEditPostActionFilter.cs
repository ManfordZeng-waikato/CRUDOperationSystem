using CRUDOperationSystem.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDOperationSystem.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesUploaderService _countriesUploaderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAdderService _countriesAdderService;
        public PersonCreateAndEditPostActionFilter(ICountriesAdderService countriesAdderService, ICountriesGetterService countriesGetterService, ICountriesUploaderService countriesService)
        {
            _countriesUploaderService = countriesService;
            _countriesAdderService = countriesAdderService;
            _countriesGetterService = countriesGetterService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countryResponses =
                   await _countriesGetterService.GetAllCountries();

                    personsController.ViewBag.Countries = countryResponses
                        .Select(c => new SelectListItem
                        {
                            Text = c.CountryName,
                            Value = c.CountryID.ToString()
                        })
                        .ToList();

                    personsController.ViewBag.Errors =
                        personsController.ModelState.Values.SelectMany(v =>
                    v.Errors).Select(e => e.ErrorMessage).ToList();
                    var personRequest =
                        context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
