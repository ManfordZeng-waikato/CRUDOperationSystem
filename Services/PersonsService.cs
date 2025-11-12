using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }


        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            /* _db.Persons.Add(person);
             _db.SaveChanges();*/
            _db.sp_InsertPerson(person);
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _db.Persons.Include("Country").ToListAsync();

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
           await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
            if (person == null)
                return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> filteredPersons = allPersons;
            if (searchBy == null || searchString == null)
            {
                return allPersons;
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    filteredPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.PersonName) ?
                    temp.PersonName.Contains(searchString,
                    StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Email) ?
                   temp.Email.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    filteredPersons = allPersons.Where(temp =>
                   (temp.DateOfBirth != null) ?
                   temp.DateOfBirth.Value.ToString("dd MMMM, yyyy").Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Gender) ?
                   temp.Gender.Equals(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Country):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Country) ?
                   temp.Country.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    filteredPersons = allPersons.Where(temp =>
                   (!string.IsNullOrEmpty(temp.Address) ?
                   temp.Address.Contains(searchString,
                   StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default:
                    filteredPersons = allPersons;
                    break;
            }
            return filteredPersons;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (sortBy == null)
                return allpersons;

            return await Task.Run(() =>
             {
                 List<PersonResponse> SortedPersons = (sortBy, sortOrder) switch
                 {
                     (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.PersonName,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.PersonName,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Email,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                      allpersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                     (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                     (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                      allpersons.OrderBy(temp => temp.Age).ToList(),

                     (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Age).ToList(),

                     (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Gender,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Gender,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Country,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Country,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.Address,
                     StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.Address,
                    StringComparer.OrdinalIgnoreCase).ToList(),

                     (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                     allpersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                     (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                    allpersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                     _ => allpersons
                 };

                 return SortedPersons;
             });
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchimgPerson =
            await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);
            if (matchimgPerson == null)
                throw new ArgumentException("Given person ID doesn't exist");

            matchimgPerson.PersonName = personUpdateRequest.PersonName;
            matchimgPerson.Gender = personUpdateRequest.Gender.ToString();
            matchimgPerson.Email = personUpdateRequest.Email;
            matchimgPerson.Address = personUpdateRequest.Address;
            matchimgPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchimgPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            matchimgPerson.CountryID = personUpdateRequest.CountryID;
            await _db.SaveChangesAsync();

            return matchimgPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person =
           await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == (personID));
            if (person == null)
                return false;

            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;
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

                    var persons = await _db.Persons
                        .Include("Country")
                        .Select(p => p.ToPersonResponse())
                        .ToListAsync();

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
                List<PersonResponse> persons = _db.Persons.Include("Country").Select(p =>
                p.ToPersonResponse()).ToList();
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
