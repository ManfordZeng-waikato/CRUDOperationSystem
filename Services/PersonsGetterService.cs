using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContract;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Globalization;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            var persons = await _personsRepository.GetAllPersons();

            return persons.Select(temp =>
           temp.ToPersonResponse()).ToList();

            // return _db.sp_GetAllPersons().Select(temp =>
            // temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;
            Person? person =
           await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
                return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");
            searchString = searchString?.Trim();
            List<Person> persons;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                persons = await _personsRepository.GetAllPersons();
            }
            else
            {
                using (SerilogTimings.Operation.Time("Time for Filtered Persons from Database"))
                {
                    persons = searchBy switch
                    {
                        nameof(PersonResponse.PersonName) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                        temp.PersonName != null && temp.PersonName.Contains(searchString)),

                        nameof(PersonResponse.Email) =>
                            await _personsRepository.GetFilteredPersons(temp =>
                                temp.Email != null && temp.Email.Contains(searchString)),

                        nameof(PersonResponse.DateOfBirth) =>
                            await GetPersonsFilteredByDateOfBirth(searchString),

                        nameof(PersonResponse.Gender) =>
                            await _personsRepository.GetFilteredPersons(temp =>
                                temp.Gender != null && temp.Gender.Contains(searchString)),

                        nameof(PersonResponse.Country) =>
                            await _personsRepository.GetFilteredPersons(temp =>
                                temp.Country != null &&
                                temp.Country.CountryName.Contains(searchString)),

                        nameof(PersonResponse.Address) =>
                            await _personsRepository.GetFilteredPersons(temp =>
                                temp.Address != null && temp.Address.Contains(searchString)),

                        _ => await _personsRepository.GetAllPersons(),
                    };
                }
            }
            _diagnosticContext.Set("PersonsCount", persons.Count);
            _diagnosticContext.Set("SearchBy", searchBy);
            _diagnosticContext.Set("SearchString", searchString);

            return persons.Select(p => p.ToPersonResponse()).ToList();
        }

        private async Task<List<Person>> GetPersonsFilteredByDateOfBirth(string searchString)
        {
            _logger.LogInformation("GetFilteredPersonsByDateOfBirth of PersonsService");

            if (int.TryParse(searchString, out int year) && searchString.Length == 4)
            {
                return await _personsRepository.GetFilteredPersons(p =>
                    p.DateOfBirth.HasValue &&
                    p.DateOfBirth.Value.Year == year);
            }

            if (DateTime.TryParse(
                    searchString,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
            {
                return await _personsRepository.GetFilteredPersons(p =>
                    p.DateOfBirth.HasValue &&
                    p.DateOfBirth.Value.Date == date.Date);
            }

            var tempList = await _personsRepository.GetFilteredPersons(p => p.DateOfBirth.HasValue);

            return tempList
                .Where(p => p.DateOfBirth!.Value
                                .ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)
                                .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            await using (StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true))
            {
                CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
                await using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration))
                {
                    csvWriter.WriteField(nameof(PersonResponse.PersonName));
                    csvWriter.WriteField(nameof(PersonResponse.Email));
                    csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                    csvWriter.WriteField(nameof(PersonResponse.Age));
                    csvWriter.WriteField(nameof(PersonResponse.Gender));
                    csvWriter.WriteField(nameof(PersonResponse.Country));
                    csvWriter.WriteField(nameof(PersonResponse.Address));
                    csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
                    await csvWriter.NextRecordAsync();

                    List<PersonResponse> persons = await GetAllPersons();
                    foreach (var person in persons)
                    {
                        csvWriter.WriteField(person.PersonName);
                        csvWriter.WriteField(person.Email);
                        csvWriter.WriteField(person.DateOfBirth?.ToString("yyyy-MM-dd") ?? "");
                        csvWriter.WriteField(person.Age);
                        csvWriter.WriteField(person.Gender);
                        csvWriter.WriteField(person.Country);
                        csvWriter.WriteField(person.Address);
                        csvWriter.WriteField(person.ReceiveNewsLetters);
                        await csvWriter.NextRecordAsync();
                    }

                    await csvWriter.FlushAsync();
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }


        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date Of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();
                foreach (PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
