using CRUDOperationSystem.StartupExtensions;
using OfficeOpenXml;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var license = new EPPlusLicense();
license.SetNonCommercialPersonal("Manford Zeng");

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

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