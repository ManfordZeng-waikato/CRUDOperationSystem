using CRUDOperationSystem.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContract;
using ServiceContracts;
using Services;

namespace CRUDOperationSystem.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            /*services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                options.RequestBodyLogLimit = 4096;
                options.ResponseBodyLogLimit = 4096;
            });
            */
            /*builder.Logging
                   .ClearProviders().AddConsole().AddDebug() ;*/

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5);

                options.Filters.Add(new ResponseHeaderFilterFactoryAttribute("My-Key-From-Global", "My-Value-From-Global", 2));
            });

            //add services into IOC container 
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            if (!env.IsEnvironment("Test"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection")!);
                });
            }
            services.AddScoped<ResponseHeaderActionFilter>();

            return services;

        }
    }
}
