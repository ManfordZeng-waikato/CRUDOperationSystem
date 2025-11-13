using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Db
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries =
            System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }


            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons =
            System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent API
            modelBuilder.Entity<Person>().Property(p =>
            p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(10)")
                .HasDefaultValue("ABC1234567");

            /* modelBuilder.Entity<Person>().HasIndex(p =>
             p.TIN).IsUnique();*/

            modelBuilder.Entity<Person>()
             .ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_TIN", "LEN([TaxIdentificationNumber]) = 10");
                });

        }
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.TIN))
            {
                person.TIN = "ABC1234567";
            }

            SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@PersonID",person.PersonID ),
                new SqlParameter("@PersonName",person.PersonName ),
                new SqlParameter("@DateOfBirth",person.DateOfBirth ),
                new SqlParameter("@Email",person.Email ),
                new SqlParameter("@Gender",person.Gender ),
                new SqlParameter("@CountryID",person.CountryID ),
                new SqlParameter("@Address",person.Address ),
                new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters ),
                new SqlParameter("@TIN", person.TIN )
            };
            return Database.ExecuteSqlRaw
                ("EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@ReceiveNewsLetters,@TIN", sqlParameters);
        }
    }
}
