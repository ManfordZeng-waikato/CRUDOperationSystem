using CRUDOperationSystem.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDOperationSystem.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
            if (parameters != null && parameters.ContainsKey("searchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
            }
            if (parameters != null && parameters.ContainsKey("searchString"))
            {
                personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
            }
            if (parameters != null && parameters.ContainsKey("sortBy"))
            {
                personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
            }
            else
            {
                personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
            }

            if (parameters != null && parameters.ContainsKey("sortOrderOptions"))
            {
                personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrderOptions"]);
            }
            else
            {
                personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
            }

            personsController.ViewBag.SearchFileds = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName) ,"Person Name" },
                { nameof(PersonResponse.Email) ,"Email" },
                { nameof(PersonResponse.DateOfBirth) ,"Date of Birth" },
                { nameof(PersonResponse.Gender) ,"Gender" },
                { nameof(PersonResponse.Country) ,"Country " },
                { nameof(PersonResponse.Address) ,"Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (searchBy != null)
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Country),nameof(PersonResponse.Address),
                    };

                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value is {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value is {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
