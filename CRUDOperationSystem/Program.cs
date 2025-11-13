using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
var license = new EPPlusLicense();
license.SetNonCommercialPersonal("Manford Zeng");


builder.Services.AddControllersWithViews();

//add services into IOC container 
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
});



/*Data Source = (localdb)\MSSQLLocalDB;
Initial Catalog = PersonsDatabase;
Integrated Security = True;
Connect Timeout = 30;
Encrypt = False;
Trust Server Certificate=False;
Application Intent = ReadWrite;
Multi Subnet Failover=False*/

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
