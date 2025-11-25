using CRUDOperationSystem.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repository;
using RepositoryContract;
using Serilog;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
var license = new EPPlusLicense();
license.SetNonCommercialPersonal("Manford Zeng");

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

/*builder.Logging
       .ClearProviders().AddConsole().AddDebug() ;*/

builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<ResponseHeaderActionFilter>(5);

    options.Filters.Add(new ResponseHeaderFilterFactoryAttribute("My-Key-From-Global", "My-Value-From-Global", 2));
});

//add services into IOC container 
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

if (builder.Environment.IsEnvironment("Test") == false)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
  {
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
  });
}

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ResponseHeaderActionFilter>();

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
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
app.UseSerilogRequestLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

//app.UseHttpLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }