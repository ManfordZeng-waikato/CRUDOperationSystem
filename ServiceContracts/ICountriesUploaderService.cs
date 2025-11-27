using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
