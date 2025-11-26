using CRUDOperationSystem.Middlewares;
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

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandlingMiddleware();
}

app.UseSerilogRequestLogging();

if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }