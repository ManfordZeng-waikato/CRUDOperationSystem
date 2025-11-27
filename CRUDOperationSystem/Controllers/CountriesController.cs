using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDOperationSystem.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesUploaderService _countriesUploaderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAdderService _countriesAdderService;
        public CountriesController(ICountriesAdderService countriesAdderService, ICountriesGetterService countriesGetterService, ICountriesUploaderService countriesUploaderService)
        {
            _countriesUploaderService = countriesUploaderService;
            _countriesAdderService = countriesAdderService;
            _countriesGetterService = countriesGetterService;
        }


        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";
                return View();
            }

            int countriesCountInserted =
            await _countriesUploaderService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{countriesCountInserted} Countries Uploaded";
            return View();
        }
    }
}
