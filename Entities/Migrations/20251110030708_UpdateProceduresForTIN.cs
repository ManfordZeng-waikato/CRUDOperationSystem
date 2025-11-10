using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProceduresForTIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                ALTER PROCEDURE [dbo].[GetAllPersons]   
                AS BEGIN
                  SELECT PersonID,PersonName,Email,DateOfBirth,Gender,CountryID,Address,ReceiveNewsLetters,TIN FROM [dbo].[Persons]
                END
            ";

            string sp_InsertPerson = @"
                ALTER PROCEDURE [dbo].[InsertPerson] 
                (@PersonID uniqueidentifier, @PersonName nvarchar(40),@Email nvarchar(40), @DateOfBirth datetime2(7), @Gender nvarchar(10), @CountryID uniqueidentifier, @Address nvarchar(200), @ReceiveNewsLetters bit,@TIN nvarchar(max) )
                AS BEGIN
                  INSERT INTO [dbo].[Persons](PersonID,PersonName,Email,DateOfBirth,Gender,CountryID,Address,ReceiveNewsLetters,TIN) VALUES(@PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@ReceiveNewsLetters,@TIN)
                END
            ";

            migrationBuilder.Sql(sp_InsertPerson);
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
               DROP PROCEDURE [dbo].[InsertPerson]
            ";
            string sp_GetAllPersons = @"
               DROP PROCEDURE [dbo].[GetAllPersons] 
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
            migrationBuilder.Sql(sp_InsertPerson);

        }
    }
}
