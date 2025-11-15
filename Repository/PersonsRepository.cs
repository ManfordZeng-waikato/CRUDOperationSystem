using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContract;
using System.Linq.Expressions;

namespace Repository
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(p => p.PersonID == personID));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _db.Persons.Include("Country")
                .FirstOrDefaultAsync(p => p.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? person_to_be_updated = await _db.Persons.FirstOrDefaultAsync(p =>
            p.PersonID == person.PersonID);

            if (person_to_be_updated == null)
                return person;

            person_to_be_updated.PersonName = person.PersonName;
            person_to_be_updated.Email = person.Email;
            person_to_be_updated.DateOfBirth = person.DateOfBirth;
            person_to_be_updated.Gender = person.Gender;
            person_to_be_updated.CountryID = person.CountryID;
            person_to_be_updated.Address = person.Address;
            person_to_be_updated.ReceiveNewsLetters = person.ReceiveNewsLetters;

            int countUpdated = await _db.SaveChangesAsync();
            return person_to_be_updated;
        }
    }
}
