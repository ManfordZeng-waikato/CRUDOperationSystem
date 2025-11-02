using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public String? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse personResponse = (PersonResponse)obj;
            return personResponse.PersonName == PersonName &&
                personResponse.Email == Email &&
                personResponse.DateOfBirth == DateOfBirth &&
                personResponse.Gender == Gender &&
                personResponse.CountryID == CountryID &&
                personResponse.Address == Address &&
                personResponse.ReceiveNewsLetters == ReceiveNewsLetters &&
                personResponse.Age == Age &&
                personResponse.PersonID == PersonID &&
                personResponse.Country == Country;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonID:{PersonID},PersonName:{PersonName}," +
                $"Email:{Email},Date Of Birth:{DateOfBirth?.ToString("dd MM yyyy")}," +
                $"Gender:{Gender},Country ID:{CountryID},Country:{Country}," +
                $"Address:{Address},Receive news letters:{ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Address = Address,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters,
            };
        }
    }


    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Address = person.Address,
                CountryID = person.CountryID,
                PersonID = person.PersonID,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Gender = person.Gender,
                Age = (person.DateOfBirth != null) ?
                Math.Round((DateTime.Now - person.DateOfBirth.Value)
               .TotalDays / 365.25) : null
            };
        }

    }
}
